using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyEffectManager : MonoBehaviour
{
    public void StartLucky(PlayerController player, TileLucky tile)
    {
        switch(tile.method)
        {
            case "PayToBank":
                {
                    PayToBank(player, tile.value, tile.percentage);
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
}
