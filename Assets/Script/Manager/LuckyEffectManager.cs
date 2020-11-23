﻿using System.Collections;
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
            Debug.Log("Chegou 1");
            player.DebitValue(value);
        }
        else
        {
            Debug.Log("Chegou 2");
            player.DebitValue((int)(player.currentMoney * ((float)percentage/100)));
        }
    }
}