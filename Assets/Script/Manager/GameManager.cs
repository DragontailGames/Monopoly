using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();

    int currentPlayer = 0;

    public CanvasManager canvasManager;

    public void Start()
    {
        StartRound(players[currentPlayer]);
    }

    public void StartRound(PlayerController player)
    {
        canvasManager.btnThrowDice.interactable = true;
        canvasManager.btnThrowDice.onClick.RemoveAllListeners();
        canvasManager.btnThrowDice.onClick.AddListener(player.StartMovePlayer);
        canvasManager.btnThrowDice.onClick.AddListener(() => canvasManager.btnThrowDice.interactable = false);
        canvasManager.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
    }

    public IEnumerator OnMovePlayer(PlayerController player, Coroutine move, bool doubleDice)
    {
        var otherP = players.FindAll(i => i != player);
        foreach(var aux in otherP)
        {
            aux.ChangeMaterial(true);
        }
        yield return move;

        foreach (var aux in otherP)
        {
            aux.ChangeMaterial(false);
        }

        if(!doubleDice)
            currentPlayer = currentPlayer + 1 < (players.Count) ? currentPlayer + 1 : 0;

        StartRound(players[currentPlayer]);
    }
}
