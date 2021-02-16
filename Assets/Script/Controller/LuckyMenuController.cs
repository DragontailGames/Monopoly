using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckyMenuController : MonoBehaviour
{
    public TextMeshProUGUI luckyDescription;

    bool clicked = false;

    public IEnumerator LuckyStart(TileLucky lucky, PlayerController player)
    {
        if(!player.botController)
            this.gameObject.SetActive(true);

        clicked = false;

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
