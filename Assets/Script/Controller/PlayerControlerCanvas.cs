using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControlerCanvas : MonoBehaviour
{
    public Image icon;

    public TextMeshProUGUI playerName;

    public TextMeshProUGUI playerMoney;

    public void ConfigureUI(Sprite icon, string playerName, int money)
    {
        //this.icon.sprite = icon;
        this.playerName.text = playerName;
        ConfigureMoney(money);
    }

    public void ConfigureMoney(int money)
    {
        if (money > 10000)
        {
            this.playerMoney.text = "$" + string.Format($"{money / 1000:#,##0K}");
        }
        else
        {
            this.playerMoney.text = "$" + money;
        }
    }
}
