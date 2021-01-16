using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;

    [HideInInspector]
    public int position = 0;

    public int dice1 = 15;//ThrowDice();

    public int dice2 = 9;//ThrowDice();

    [HideInInspector]
    public bool doubleDice = false;

    public void Awake()
    {
        if(!playerController)
        {
            playerController = this.GetComponent<PlayerController>();
        }
    }

    public void StartMovePlayer()
    {
        dice1 = playerController.ThrowDice();
        dice2 = playerController.ThrowDice();

        doubleDice = dice1 == dice2;

        if(doubleDice)
        {
            playerController.doubleRow++;
            if(playerController.doubleRow>2)
            {
                playerController.doubleRow = 0;
                playerController.GotoJail();

                playerController.manager.NextPlayer();

                StartCoroutine(playerController.manager.StartRound());
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
            TileController tile = playerController.boardController.tileControllers.Find(t => t.index == i);

            if (i + 1 >= playerController.boardController.tileControllers.Count)
            {
                var tempDest = dest - i;
                i = -1;
                dest = tempDest;
            }

            Vector3 targetPos = tile.transform.position;
            targetPos.y = this.transform.position.y;

            yield return Move(targetPos);

            position = i;
            playerController.currentTile = tile;
            yield return tile.OnPlayerPass(playerController);
        }

        yield return playerController.currentTile.OnPlayerStop(playerController);

    }

    public IEnumerator Move(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            float step = 3.5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator RepositionInTile(int index, int amount)
    {
        yield return Move(GetRepositionInTile(index,amount));
    }

    public Vector3 GetRepositionInTile(int index, int amount)
    {
        index += 1;
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

}
