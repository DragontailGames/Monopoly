using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMoveController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;

    [HideInInspector]
    public int position = 0;

    [HideInInspector]
    public bool doubleDice = false;

    public void Awake()
    {
        if(!playerController)
        {
            playerController = this.GetComponent<PlayerController>();
        }
    }

    [PunRPC]
    public void StartMovePlayer_CMD(int dice1, int dice2)
    {
        doubleDice = dice1 == dice2;

        if(doubleDice)
        {
            playerController.doubleRow++;
            if(playerController.doubleRow>2)
            {
                playerController.doubleRow = 0;
                playerController.GotoJail();

                playerController.photonView.RPC("NextPlayer_CMD", RpcTarget.All);
                return;
            }
        }
        else
        {
            playerController.doubleRow = 0;
        }

        int valueDice = dice1 + dice2;

        if (playerController.inJail && doubleDice)
        {
            playerController.inJail = false;
        }
        if (!playerController.inJail)
        {
            StartCoroutine(playerController.manager.OnMovePlayer(this, MovePlayer(valueDice), doubleDice));
        }
    }

    [PunRPC]
    public void MovePlayer_CMD(int valueDice)
    {
        StartCoroutine(MovePlayer(valueDice));
    }

    public IEnumerator MovePlayer(int valueDice)
    {
        int dest = valueDice + position;
        for (int i = position + 1; i <= dest; i++)
        {
            if (i < dest)
            {
                playerController.Animate_Walk();
            }
            TileController tile = playerController.boardController.tileControllers.Find(t => t.index == i);

            if (i + 1 >= playerController.boardController.tileControllers.Count)
            {
                var tempDest = dest - i;
                i = -1;
                dest = tempDest;
            }

            Vector3 newPos = tile.GetPosition();

            yield return Move(OffsetRotation(newPos));

            position = i;
            playerController.photonView.RPC("SetCurrentTile_CMD", RpcTarget.All, tile.index);
            yield return tile.OnPlayerPass(playerController);
        }

        yield return playerController.currentTile.OnPlayerStop(playerController);

    }

    public IEnumerator Move(Vector3 targetPos)
    {
        int counts = 0;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            if(counts>200 || Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                break;
            }
            float step = 3.5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            counts++;
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator RepositionInTile(int index, int amount)
    {
        if(amount == 2)
        {
            yield return Move(GetRepositionInTile(index,amount));
        }
        else if(amount > 2)
        {
            yield return SetupIcons(index+1);
        }
        else
        {
            yield break;
        }
    }

    public Vector3 GetRepositionInTile(int index, int amount)
    {
        Vector3 newPos = playerController.currentTile.transform.position;

        newPos.y = this.transform.position.y;

        float fixPos = 0.25f;

        if (amount == 2)
        {
            if (index == 0)
            {
                if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 80, 100))
                {
                    newPos.z += fixPos;
                }
                else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 170, 190))
                {
                    newPos.x -= fixPos;
                }
                else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 260, 280))
                {
                    newPos.z -= fixPos;
                }
                else
                {
                    newPos.x += fixPos;
                }
            }
            else
            {
                if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 80, 100))
                {
                    newPos.z -= fixPos;
                }
                else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 170, 190))
                {
                    newPos.x += fixPos;
                }
                else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 260, 280))
                {
                    newPos.z += fixPos;
                }
                else
                {
                    newPos.x -= fixPos;
                }
            }
        }
        return OffsetRotation(newPos);
    }

    public Vector3 OffsetRotation(Vector3 newPos)
    {
        if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 80, 100))
        {
            newPos.x += playerController.currentTile.offsetZ;
        }
        else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 170, 190))
        {
            newPos.z -= playerController.currentTile.offsetZ;
        }
        else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 260, 280))
        {
            newPos.x += playerController.currentTile.offsetZ;
        }
        else
        {
            newPos.z -= playerController.currentTile.offsetZ;
        }
        return newPos;
    }

    public IEnumerator SetupIcons(int index)
    {
        playerController.photonView.RPC("EnableIcon", RpcTarget.All, index);

        yield return new WaitForEndOfFrame();
    }

    [PunRPC]
    public void EnableIcon(int index)
    {
        playerController.transform.position = GetRepositionInTile(0, 1);
        float offsetY = 3f;

        this.transform.Find("Model").gameObject.SetActive(false);

        GameObject icon = playerController.transform.Find("Icon").gameObject;
        icon.SetActive(true);

        icon.transform.localPosition = Vector3.up * ((index - 1) * offsetY);
    }

    [PunRPC]
    public void NextPlayer_CMD() 
    {
        playerController.manager.NextPlayer();
        StartCoroutine(playerController.manager.StartRound());
    }

}
