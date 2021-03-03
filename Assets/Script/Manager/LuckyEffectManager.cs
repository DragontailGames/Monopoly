using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LuckyEffectManager : MonoBehaviour
{
    public BoardController board;

    private TileLucky tileLucky;
    private PlayerController playerController;

    public void StartLucky(PlayerController player, TileLucky tile)
    {
        tileLucky = tile;
        playerController = player;

        switch (tile.method)
        {
            case "PayToBank":
                {
                    Debug.Log("Sorte ou revez PayToBank " + tile.text);
                    PayToBank(player, tile.value, tile.percentage);
                    return;
                }
            case "GoTo":
                {
                    Debug.Log("Sorte ou revez GOTO " + tile.tileIndex);
                    TileController tileController = board.tileControllers.Find(n => n.index.Equals(tile.tileIndex));
                    GoTo(player, tileController);
                    return;
                }
            case "FreeBoat":
                {
                    Debug.Log("Sorte ou revez Barquinho " + tile.text);
                    FreeBoat(player);
                    return;
                }
            case "ChooseProperty":
                {
                    Debug.Log("Sorte ou revez Propertie " + tile.text);
                    ChooseProperty(player);
                    return;
                }
            case "StayAway":
                {
                    player.stayAway = true;
                    return;
                }
            default:
                {
                    Debug.LogError("Metodo nao encontrado");
                    return;
                }
        }
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
    }

    private void GoTo(PlayerController player, TileController tile)
    {
        player.GotoTile(tile);
    }

    private void FreeBoat(PlayerController player)
    {
        player.freeBoat = true;
    }

    private void ChooseProperty(PlayerController player)
    {
        UnityAction<TileController> action;
        if (tileLucky.percentage != 0)
        {
            action = ChooseProperty_ChancheValueByTime;
        }
        else
        {
            action = ChooseProperty_BackToBank;
        }
        board.SetupPropertieLucky(player, action, tileLucky.ownerTileEffect);
    }

    private void ChooseProperty_ChancheValueByTime(TileController tile)
    {
        if(tileLucky.percentage != 0)
        {
            TileController_Country countryTile = (TileController_Country)tile;
            countryTile.multiplier = tileLucky.percentage;
        }
    }

    private void ChooseProperty_BackToBank(TileController tile)
    {
        TileController_Country countryTile = (TileController_Country)tile;
        countryTile.Owner = null;
        countryTile.UpgradeLevel(0, playerController);
    }
}
