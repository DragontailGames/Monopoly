using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckyMenuController : MonoBehaviour
{
    public TextMeshProUGUI luckyDescription;

    bool clicked = false;

    public Sprite iconLuck, iconReverse;

    public Image icon;

    public IEnumerator LuckyStart(TileLucky lucky, PlayerController player)
    {
        if(!player.botController)
            this.gameObject.SetActive(true);

        clicked = false;

        icon.sprite = lucky.luckType == EnumDt.luckType.luck ? iconLuck : iconReverse;

        luckyDescription.text = lucky.text;

        if (player.botController)
        {
            yield return player.botController.ExecuteAction(() => { clicked = true; });
        }

        yield return new WaitUntil(() => clicked == true);
    }

    public void ClickBackButton()
    {
        clicked = true;
        this.gameObject.SetActive(false);
    }
}
