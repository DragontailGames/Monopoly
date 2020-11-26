using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerCanvas : MonoBehaviour
{
    public Image icon;

    public TextMeshProUGUI playerName;

    public TextMeshProUGUI playerMoney;

    public void ConfigureUI(Sprite icon, string playerName, int money)
    {
        //this.icon.sprite = icon;
        this.playerName.text = playerName;
        this.playerMoney.text = "$" + Math.ConfigureMoney(money);
    }

    public void UpdateMoney(int money)
    {
        this.playerMoney.text = "$" + Math.ConfigureMoney(money);
    }

    public void StartRound()
    {

    }

    public void EndRound()
    {

    }
}
