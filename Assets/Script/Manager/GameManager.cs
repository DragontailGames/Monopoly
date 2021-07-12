using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Dragontailgames.Utils;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();

    public BoardController board;

    public int currentPlayer = 0;

    public CanvasManager canvasManager;

    Dictionary<EnumDt.tradingBlock, List<TileController_Country>> countryController = new Dictionary<EnumDt.tradingBlock, List<TileController_Country>>();

    public NetworkManager networkManager;

    public GameObject dice1, dice2, dices;

    private int roundCount = 0;

    public Color[] playerColors = new Color[4];

    public List<PlayerDg> availablePlayers = new List<PlayerDg>();

    public GameObject backtoMenu;

    public void Start()
    {
        board.manager = this;
    }

    public void NewPlayer(PlayerController newPlayer)
    {
        players.Add(newPlayer);
    }

    bool setup = false;

    public void Update()
    {
        if(!setup && players.Count == networkManager.GetPlayerNetworkCount)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlayerController aux = players[i];
                aux.photonView.RPC("SetCurrentTile_CMD", RpcTarget.All, networkManager.startTile.GetComponent<TileController>().index);
                //StartCoroutine(aux.moveController.RepositionInTile(i-1, players.Count));
            }
            StartGame();
            setup = true;
        }
    }

    public void StartGame()
    {
        players.Sort((a, b) => a.playerNumber.CompareTo(b.playerNumber));

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

        if (player.player == null || (!player.player.IsLocal && !player.botController))
        {
            yield break;
        }

        if (MessageManager.Instance.TextShowing() && !player.botController)
        {
            yield return new WaitForSeconds(3.0f);
            MessageManager.Instance.HiddenText();
        }

        if (players.Count == 1)
        {
            players[0].WinGame();
            yield break; 
        }

        var playersInSamePos = ListTurnOrderPlayers().FindAll(n => n.currentTile == player.currentTile);
        if (playersInSamePos.Count > 1)
        {
            for (int i = 0; i < playersInSamePos.Count; i++)
            {
                PlayerController aux = (PlayerController)playersInSamePos[i];
                yield return aux.moveController.RepositionInTile(i-1, playersInSamePos.Count);
            }
        }

        player.photonView.RPC("EnableModel_CMD", RpcTarget.All);

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
            if (player.inJail)
            {
                player.photonView.RPC("NextPlayer_CMD", RpcTarget.All, true);
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
               // player.btnThrowDice.image.color = player.GetComponent<MeshRenderer>().materials[0].color;
            }
            else
            {
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
            player.playerController.photonView.RPC("NextPlayer_CMD", RpcTarget.All, true);
        }
        else if(doubleDice)
        {
            StartCoroutine(player.playerController.manager.StartRound());
        }
        else if (playerDefetead)
        {
            playerDefetead = false;
            DestroyImmediate(player.gameObject);
            StartCoroutine(StartRound());
        }
    }

    public bool playerDefetead = false;

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
        var playersInSamePos = ListTurnOrderPlayers().FindAll(n => n.currentTile == newPlayer.playerController.currentTile);
        if(playersInSamePos.Count>1)
        {
            for (int i = 0; i < playersInSamePos.Count; i++)
            {
                PlayerController aux = (PlayerController)playersInSamePos[i];
                yield return aux.moveController.RepositionInTile(i, playersInSamePos.Count);
            }
            //yield return newPlayer.RepositionInTile(playersInSamePos.Count-1, playersInSamePos.Count);
        }
    }

    public void NextPlayer()
    {
        currentPlayer = currentPlayer + 1 < (players.Count) ? currentPlayer + 1 : 0;

        if(currentPlayer==0)
        {
            NextRound();
        }
    }

    public void NextRound()
    {
        roundCount++;
        foreach(var aux in board.tileControllers)
        {
            if(aux.GetType() == typeof(TileController_Country))
            {
                var tile = aux as TileController_Country;
                tile.roundsWithMultiplier = Mathf.Clamp((tile.roundsWithMultiplier-1), 0, 3);
            }
        }
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

    public IEnumerator RollDice(int dice1Value, int dice2Value, int playerNumber, UnityEngine.Events.UnityAction afterDiceRoll = null)
    {
        int yStart =315;
        float correction = 90 * (playerNumber - 1);
        dices.transform.rotation = Quaternion.Euler(new Vector3(0,yStart - correction, 0));

        Animator dice1Animator = dice1.GetComponentInChildren<Animator>();
        dice1.SetActive(true);
        dice1Animator.SetInteger("RollNumber", dice1Value);

        Animator dice2Animator = dice2.GetComponentInChildren<Animator>();
        dice2.SetActive(true);
        dice2Animator.SetInteger("RollNumber", dice2Value);

        yield return new WaitForSeconds(0.4f * (dice1Value + dice2Value) + 1.0f);

        dice1.SetActive(false);
        dice2.SetActive(false);

        afterDiceRoll?.Invoke();
    }

    public List<PlayerController> ListTurnOrderPlayers()
    {
        List<PlayerController> pTempList = new List<PlayerController>();

        for(int i =0;i<players.Count;i++)
        {
            int offsetIndex = currentPlayer + i;
            if(offsetIndex>players.Count-1)
            {
                offsetIndex -= players.Count;
            }
            pTempList.Add(players[offsetIndex]);
        }

        return pTempList;
    }

    public void EndGame()
    {
        backtoMenu.SetActive(true);
    }

    public void GotoMenu()
    {
        SceneLoadManager.instance.gotoMenu();
    }
}
