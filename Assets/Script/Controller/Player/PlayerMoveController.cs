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

    public void StartMovePlayer(int dice1, int dice2)
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

    public IEnumerator MovePlayer(int valueDice)
    {
        int dest = valueDice + position;
        for (int i = position + 1; i <= dest; i++)
        {
            playerController.Animate_Walk();
            TileController tile = playerController.boardController.tileControllers.Find(t => t.index == i);

            if (i + 1 >= playerController.boardController.tileControllers.Count)
            {
                var tempDest = dest - i;
                i = -1;
                dest = tempDest;
            }

            Vector3 newPos = tile.GetPosition();

            if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 80, 100))
            {
                newPos.x += tile.offsetZ;
            }
            else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 170, 190))
            {
                newPos.z -= tile.offsetZ;
            }
            else if (MathDt.IsBetween(this.transform.rotation.eulerAngles.y, 260, 280))
            {
                newPos.x += tile.offsetZ;
            }
            else
            {
                newPos.z -= tile.offsetZ;
            }

            yield return Move(newPos);

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
            if(counts>200 && Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                break;
            }
            playerController.photonView.RPC("Move_CMD", RpcTarget.All, targetPos);
            counts++;
            yield return new WaitForSeconds(0.001f);
        }
    }

    [PunRPC]
    public void Move_CMD(Vector3 targetPos)
    {
        float step = 3.5f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    public IEnumerator RepositionInTile(int index, int amount)
    {
        yield return Move(GetRepositionInTile(index,amount));
    }

    public Vector3 GetRepositionInTile(int index, int amount)
    {
        Vector3 newPos = playerController.currentTile.transform.position;

        newPos.z += playerController.currentTile.offsetZ;
        newPos.y = this.transform.position.y;

        if (amount > 1)
        {
            if (index % 2 == 0)
            {
                newPos.z -= 0.2f;
            }
            else
            {
                newPos.z += 0.2f;
            }
            if (amount > 2)
            {
                if (index < 2)
                {
                    newPos.x -= 0.2f;
                }
                else
                {
                    newPos.x += 0.2f;
                }
            }
        }
        return newPos;
    }

    [PunRPC]
    public void NextPlayer_CMD() 
    {
        playerController.manager.NextPlayer();
        StartCoroutine(playerController.manager.StartRound());
    }

}
