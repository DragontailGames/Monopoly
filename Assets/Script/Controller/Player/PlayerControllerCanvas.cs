﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerCanvas : MonoBehaviour
{
    public Image icon;

    public TextMeshProUGUI playerName;

    public TextMeshProUGUI playerMoney;

    public PlayerController player;

    int interateValue = 3000;

    public void ConfigureUI(Sprite icon, string playerName, int money)
    {
        //this.icon.sprite = icon;
        this.playerName.text = playerName;
        this.playerMoney.text = "$" + Math.ConfigureMoney(money);
    }

    public IEnumerator DebitAnimation(int value, int totalValue, IEnumerator startBefore = null)
    {
        if (value >= interateValue)
        {
            value -= interateValue;
            totalValue -= interateValue;
        }
        else
        {
            totalValue -= value;
            value = 0;
        }
        yield return UpdateMoney(totalValue);
        if (value > 0)
        {
            StartCoroutine(DebitAnimation(value, totalValue));
        }
        else
        {
            CheckValueIsWrong();
        }
    }

    public IEnumerator CreditAnimation(int value, int totalValue)
    {
        if (value >= interateValue)
        {
            value -= interateValue;
            totalValue += interateValue;
        }
        else
        {
            totalValue += value;
            value = 0;
        }
        yield return UpdateMoney(totalValue);
        if(value>0)
        {
            StartCoroutine(CreditAnimation(value, totalValue));
        }
        else
        {
            CheckValueIsWrong();
        }
    }

    public void CheckValueIsWrong()
    {
        if(player.currentMoney>exibedMoney)
        {
            int value = player.currentMoney - exibedMoney;
            StartCoroutine(CreditAnimation(value, exibedMoney));
        }
        else if(player.currentMoney<exibedMoney)
        {
            int value = exibedMoney - player.currentMoney;
            StartCoroutine(DebitAnimation(value, exibedMoney));
        }
    }

    public int exibedMoney = 0;

    public IEnumerator UpdateMoney(int money)
    {
        exibedMoney = money;
        this.playerMoney.text = "$" + Math.ConfigureMoney(money);
        yield return true;
    }

    public void StartRound()
    {

    }

    public void EndRound()
    {

    }

    public void DeclareBankruptcy()
    {
        var imgs = this.GetComponentsInChildren<Image>();
        foreach(Image aux in imgs)
        {
            Color nColor = aux.color;
            nColor.a = 0.5f;
            aux.color = nColor;
        }
    }
}
