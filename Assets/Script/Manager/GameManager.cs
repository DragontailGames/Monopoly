using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();

    public BoardController board;

    int currentPlayer = 0;

    public CanvasManager canvasManager;

    public void Start()
    {
        StartRound(players[currentPlayer]);
    }

    public void StartRound(PlayerController player)
    {
        if (player.canTeleport)
        {
            board.SetupTeleportBoard(player);
            canvasManager.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
        }
        else
        {
            canvasManager.btnThrowDice.interactable = true;
            canvasManager.btnThrowDice.onClick.RemoveAllListeners();
            canvasManager.btnThrowDice.onClick.AddListener(player.StartMovePlayer);
            canvasManager.btnThrowDice.onClick.AddListener(() => canvasManager.btnThrowDice.interactable = false);
            canvasManager.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
        }
    }

    public IEnumerator OnMovePlayer(PlayerController player, Coroutine move, bool doubleDice)
    {
        var otherP = players.FindAll(i => i != player);
        foreach(var aux in otherP)
        {
            aux.ChangeMaterial(true);
        }
        yield return move;

        yield return TestPlayerOnSameHouse(player);

        foreach (var aux in otherP)
        {
            aux.ChangeMaterial(false);
        }

        if(!doubleDice)
            currentPlayer = currentPlayer + 1 < (players.Count) ? currentPlayer + 1 : 0;

        StartRound(players[currentPlayer]);
    }

    public IEnumerator TestPlayerOnSameHouse(PlayerController newPlayer)
    {
        //Op1 Movimento no mesmo bloco
        var playersInSamePos = players.FindAll(n => n.position == newPlayer.position);
        if(playersInSamePos.Count>1)
        {
            for (int i = 0; i < playersInSamePos.Count; i++)
            {
                PlayerController aux = (PlayerController)playersInSamePos[i];
                yield return aux.RepositionInTile(i, playersInSamePos.Count);
            }
            yield return newPlayer.RepositionInTile(playersInSamePos.Count-1, playersInSamePos.Count);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
