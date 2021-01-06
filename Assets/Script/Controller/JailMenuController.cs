using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JailMenuController : MonoBehaviour
{
    public Button btnPayJailPrice;

    public Button btnTryDice;

    bool clicked = false;

    public IEnumerator ShowCanvasPlayer(PlayerController playerController)
    {
        this.gameObject.SetActive(true);
        clicked = false;

        btnPayJailPrice.GetComponentInChildren<TextMeshProUGUI>().text = "Pay<br><size=32>" + MathDt.ConfigureMoney(MathDt.jailPrice) + "</size>";
        btnPayJailPrice.interactable = playerController.walletController.currentMoney >= MathDt.jailPrice;


        btnPayJailPrice.onClick.RemoveAllListeners();
        btnPayJailPrice.onClick.AddListener(() =>
        {
            clicked = true;
            playerController.walletController.DebitValue(MathDt.jailPrice);
            playerController.inJail = false;
            this.gameObject.SetActive(false);
        });

        btnTryDice.onClick.RemoveAllListeners();
        btnTryDice.onClick.AddListener(() =>
        {
            clicked = true;
            int dice1 = playerController.ThrowDice();
            int dice2 = playerController.ThrowDice();

            playerController.inJail = dice1 != dice2;
            this.gameObject.SetActive(false);
        });


        yield return new WaitUntil(() => clicked == true);
    }
}
