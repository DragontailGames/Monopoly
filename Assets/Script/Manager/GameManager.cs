using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();

    public BoardController board;

    [HideInInspector]
    public int currentPlayer = 0;

    public CanvasManager canvasManager;

    public void Start()
    {
        players.Sort((a, b) => (a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())));
        StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        PlayerController player = players[currentPlayer];
        if (player.inJail)
        {
            player.jailRow++;
            if(player.jailRow>2)
            {
                player.inJail = false;
                StartCoroutine(StartRound());
                yield break;
            }
            yield return canvasManager.jailMenuController.ShowCanvasPlayer(player);
            if(player.inJail)
            {
                NextPlayer();
                StartCoroutine(ConfigDice(players[currentPlayer]));
            }
            else
            {
                StartCoroutine(StartRound());
            }
        }
        else
        {
            if (player.canTravel)
            {
                board.SetupTravelBoard(player);
                canvasManager.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
            }
            else
            {
                StartCoroutine(ConfigDice(player));
            }
        }
    }

    public IEnumerator OnMovePlayer(PlayerMoveController player, IEnumerator move, bool doubleDice)
    {
        var otherP = players.FindAll(i => i != player.playerController);
        foreach (var aux in otherP)
        {
            aux.ChangeMaterial(true);
        }
        yield return move;

        yield return player.playerController.CheckBankruptcy();

        yield return TestPlayerOnSameHouse(player);

        ResetTransparentMaterial();

        if (!doubleDice)
            NextPlayer();

        StartCoroutine(StartRound());
    }

    public void ResetTransparentMaterial()
    {
        foreach (var aux in players)
        {
            aux.ChangeMaterial(false);
        }
    }

    public IEnumerator TestPlayerOnSameHouse(PlayerMoveController newPlayer)
    {
        //Op1 Movimento no mesmo bloco
        var playersInSamePos = players.FindAll(n => n.moveController.position == newPlayer.position);
        if(playersInSamePos.Count>1)
        {
            for (int i = 0; i < playersInSamePos.Count; i++)
            {
                PlayerController aux = (PlayerController)playersInSamePos[i];
                yield return aux.moveController.RepositionInTile(i, playersInSamePos.Count);
            }
            yield return newPlayer.RepositionInTile(playersInSamePos.Count-1, playersInSamePos.Count);
        }
    }

    public IEnumerator ConfigDice(PlayerController player)
    {
        yield return new WaitForSeconds(0.2f);
        canvasManager.btnThrowDice.interactable = true;
        canvasManager.btnThrowDice.onClick.RemoveAllListeners();
        canvasManager.btnThrowDice.onClick.AddListener(player.moveController.StartMovePlayer);
        canvasManager.btnThrowDice.onClick.AddListener(() => canvasManager.btnThrowDice.interactable = false);
        canvasManager.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
    }

    public void NextPlayer()
    {
        currentPlayer = currentPlayer + 1 < (players.Count) ? currentPlayer + 1 : 0;
    }

    public void BeforePlayer()
    {
        currentPlayer = currentPlayer - 1 > 0 ? currentPlayer - 1 : players.Count;
    }
}
