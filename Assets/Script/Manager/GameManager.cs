using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();

    public BoardController board;

    public int currentPlayer = 0;

    public CanvasManager canvasManager;

    Dictionary<EnumDt.tradingBlock, List<TileController_Country>> countryController = new Dictionary<EnumDt.tradingBlock, List<TileController_Country>>();

    public NetworkManager networkManager;

    public void Start()
    {
        board.manager = this;
    }

    public void NewPlayer(PlayerController newPlayer)
    {
        players.Add(newPlayer);

        foreach (var aux in players)
        {
            if (networkManager.IsMaster)
            {
                aux.photonView.RPC("SetCurrentTile_CMD", Photon.Pun.RpcTarget.All, networkManager.startTile.GetComponent<TileController>().index);
                StartCoroutine(aux.moveController.RepositionInTile(aux.playerNumber, players.Count));
            }
        }

        if (players.Count == networkManager.GetPlayerNetworkCount)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        players.Sort((a, b) => (a.playerNumber.CompareTo(b.playerNumber)));

        foreach (var aux in board.tileControllers)
        {
            if (aux.GetType() == typeof(TileController_Country))
            {
                var tcCountry = aux as TileController_Country;
                var tbCountry = aux.tile as TileBuyable_Country;
                if (countryController.ContainsKey(tbCountry.tradingBlock))
                {
                    countryController[tbCountry.tradingBlock].Add(tcCountry);
                }
                else
                {
                    countryController.Add(tbCountry.tradingBlock, new List<TileController_Country>()
                    {
                        tcCountry
                    });
                }
            }
        }

        StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        PlayerController player = players[currentPlayer];

        if(!player.player.IsLocal)
        {
            yield break;
        }

        if (players.Count==1)
        {
            players[0].WinGame();
            yield break; 
        }
        Debug.Log("Começou");
        if (player.inJail)
        {
            Debug.Log("Na cadeia");
            player.jailRow++;
            if(player.jailRow>2)
            {
                player.inJail = false;
                StartCoroutine(StartRound());
                yield break;
            }
            yield return canvasManager.jailMenuController.ShowCanvasPlayer(player);
            Debug.Log("Teste 1 " + player.inJail);
            if (player.inJail)
            {
                Debug.Log("Teste 2 ");
                player.photonView.RPC("NextPlayer_CMD", RpcTarget.All);
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
                player.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
            }
            else
            {
                Debug.Log("Chamou os dadinhos no meu pau");
                StartCoroutine(player.ConfigDice());
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

        yield return player.playerController.walletController.CheckBankruptcy();

        yield return TestPlayerOnSameHouse(player);

        ResetTransparentMaterial();

        if (!doubleDice && !playerDefetead)
        {
            player.playerController.photonView.RPC("NextPlayer_CMD", Photon.Pun.RpcTarget.All);
        }
        else
        {
            if (playerDefetead)
            {
                playerDefetead = false;
                DestroyImmediate(player.gameObject);
            }
            StartCoroutine(StartRound());
        }
    }

    bool playerDefetead = false;

    public void PlayerDefeated()
    {
        playerDefetead = true;
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

    public void NextPlayer()
    {
        currentPlayer = currentPlayer + 1 < (players.Count) ? currentPlayer + 1 : 0;
    }
    
    public void CheckWinSide(PlayerController playerController)
    {
        bool win = false;

        if (!win)
        {
            ProcessSideTile(1, 8, playerController, out win);
        }
        if (!win)
            ProcessSideTile(9, 16, playerController, out win);
        if (!win)
            ProcessSideTile(17, 24, playerController, out win);
        if (!win)
            ProcessSideTile(25, 32, playerController, out win);

        if(win)
        {
           playerController.WinGame();
        }
    }

    public void ProcessSideTile(int start, int limit, PlayerController playerController, out bool win)
    {
        for (int i = start; i < limit; i++)
        {
            if (board.tileControllers[i].GetType() == typeof(TileController_Buyable) ||
                board.tileControllers[i].GetType() == typeof(TileController_Country) ||
                board.tileControllers[i].GetType() == typeof(TileController_Wonders))
            {
                var tb = board.tileControllers[i] as TileController_Buyable;

                if (playerController != tb.Owner || tb.Owner == null)
                {
                    win = false;
                    return;
                }
            }
        }
        win = true;
    }

    public void CheckWinBlocks(PlayerController playerController)
    {
        int totalBlocks = 0;

        foreach(var aux in countryController.Keys)
        {
            bool fullBlock = true;
            foreach(var temp in countryController[aux])
            {           
                if (playerController != temp.Owner)
                {
                    fullBlock = false;
                    break;
                }
            }
            if (fullBlock)
            {
                totalBlocks++;
            }
        }

        if (totalBlocks>=4)
        {
            playerController.WinGame();
            return;
        }
    }
}
