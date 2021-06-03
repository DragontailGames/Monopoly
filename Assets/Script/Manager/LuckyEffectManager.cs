using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LuckyEffectManager : MonoBehaviour
{
    public BoardController board;

    private TileLucky tileLucky;

    private PlayerController playerController;

    private bool clicked;

    public IEnumerator StartLucky(PlayerController player, TileLucky tile)
    {
        tileLucky = tile;
        playerController = player;

        switch (tile.method)
        {
            case "PayToBank":
                {
                    Debug.Log("Sorte ou revez PayToBank " + tile.text);
                    PayToBank(player, tile.value, tile.percentage);
                    break ;
                }
            case "GoTo":
                {
                    Debug.Log("Sorte ou revez GOTO " + tile.tileIndex);
                    TileController tileController = board.tileControllers.Find(n => n.index.Equals(tile.tileIndex));
                    GoTo(player, tileController, tile.tileName);
                    break;
                }
            case "FreeBoat":
                {
                    Debug.Log("Sorte ou revez Barquinho " + tile.text);
                    FreeBoat(player);
                    break;
                }
            case "ChooseProperty":
                {
                    Debug.Log("Sorte ou revez Propertie " + tile.text);
                    ChooseProperty(player);
                    break;
                }
            case "StayAway":
                {
                    player.stayAway = true;
                    clicked = true;
                    break;
                }
            default:
                {
                    Debug.LogError("Metodo nao encontrado");
                    clicked = true;
                    break;
                }
        }

        string msg = tile.feedbackText.Replace("[Jogador]", $"{player.name}");
        player.LogMessagePlayer(msg, false);

        yield return new WaitUntil(() => clicked == true);
    }

    private void PayToBank(PlayerController player, int value, int percentage)
    {
        if (value != 0)
        {
            player.walletController.DebitValue(value);
        }
        else
        {
            player.walletController.DebitValue((int)(player.walletController.currentMoney * ((float)percentage/100)));
        }

        clicked = true;
    }

    private void GoTo(PlayerController player, TileController tile, string tileName)
    {
        player.GotoTile(tile, tileName, tileLucky.vacation, tileLucky.jail);

        clicked = true;
    }

    private void FreeBoat(PlayerController player)
    {
        player.photonView.RPC("SetupFreeBoat_CMD", Photon.Pun.RpcTarget.All, true);

        clicked = true;
    }

    private void ChooseProperty(PlayerController player)
    {
        if (player.properties.Count > 0)
        {
            UnityAction<TileController> action;
            bool onlyCountry = false;
            if (tileLucky.percentage != 0)
            {
                action = ChooseProperty_ChangeValueByTime;
                onlyCountry = true;
            }
            else
            {
                action = ChooseProperty_BackToBank;
            }
            board.SetupPropertieLucky(player, action, tileLucky.ownerTileEffect, onlyCountry);
        }
        else
        {
            clicked = true;
        }
    }

    private void ChooseProperty_ChangeValueByTime(TileController tile)
    {
        if(tile)
        { 
            if (tileLucky.percentage != 0)
            {
                TileController_Country countryTile = (TileController_Country)tile;
                countryTile.roundsWithMultiplier = 3;
                countryTile.multiplier = tileLucky.percentage;
            }
        }

        board.ResetBoard();
        clicked = true;
    }

    private void ChooseProperty_BackToBank(TileController tile)
    {
        if (tile)
        {
            TileController_Country countryTile = (TileController_Country)tile;
            countryTile.Owner = null;
            countryTile.UpgradeLevel(0, playerController);
            countryTile.multiplier = 100;
            countryTile.roundsWithMultiplier = 0;
        }

        board.ResetBoard();
        clicked = true;
    }
}
