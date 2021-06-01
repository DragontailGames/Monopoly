using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JailMenuController : MonoBehaviour
{
    public Button btnPayJailPrice;

    public Button btnTryDice;

    public GameObject objFreeBoat;

    bool clicked = false;

    public IEnumerator ShowCanvasPlayer(PlayerController playerController)
    {
        objFreeBoat.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        if (playerController.freeBoat)
        {
            ConfigFreeBoat(playerController);
        }

        if (!playerController.botController)
        {
            this.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        }

        clicked = false;

        btnPayJailPrice.GetComponentInChildren<TextMeshProUGUI>().text = MathDt.ConfigureMoney(MathDt.jailPrice);
        btnPayJailPrice.interactable = playerController.walletController.currentMoney >= MathDt.jailPrice;

        btnPayJailPrice.onClick.RemoveAllListeners();
        btnPayJailPrice.onClick.AddListener(() =>
        {
            clicked = true;
            playerController.walletController.DebitValue(MathDt.jailPrice);
            playerController.LogMessagePlayer($"{playerController.name} pagou por um resgate urgente para sair da prisão!", false);
            playerController.inJail = false;
            this.gameObject.SetActive(false);
        });

        btnTryDice.onClick.RemoveAllListeners();
        btnTryDice.onClick.AddListener(() =>
        {
            int dice1 = playerController.ThrowDice();
            int dice2 = playerController.ThrowDice();

            this.transform.GetChild(0).gameObject.SetActive(false);

            UnityEngine.Events.UnityAction afterEvent = () =>
            {
                if (dice1 == dice2)
                {
                    playerController.LogMessagePlayer($"{playerController.name} Saiu da prisao", true);//Pedro
                    playerController.inJail = false;
                }
                else
                {
                    playerController.LogMessagePlayer($"{playerController.name} está perdido no triângulo das bermudas! Restam {3 - playerController.jailRow} turnos para ser resgatado!", true);
                }

                clicked = true;
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            };

            StartCoroutine(playerController.manager.RollDice(dice1, dice2, playerController.playerNumber, afterEvent));
        });

        //BOT
        if (playerController.botController)
        {
            if (playerController.freeBoat)
            {
                yield return playerController.botController.ExecuteAction(() =>
                {
                    clicked = true;
                    playerController.inJail = false;
                    playerController.freeBoat = false;
                    playerController.LogMessagePlayer($"{playerController.name} utilizou um sinalizador para sair da prisão!", false);
                });
            }
            else
            {
                yield return playerController.botController.ExecuteAction(() =>
                {
                    clicked = true;
                    int dice1 = playerController.ThrowDice();
                    int dice2 = playerController.ThrowDice();

                    StartCoroutine(playerController.manager.RollDice(dice1, dice2, playerController.playerNumber));

                    if(dice1 == dice2)
                    {
                        playerController.LogMessagePlayer($"{playerController.name} ", true);//Pedro
                        playerController.inJail = false;
                    }
                    else
                    {
                        playerController.LogMessagePlayer($"{playerController.name} está perdido no triângulo das bermudas! Restam {3 - playerController.jailRow} turnos para ser resgatado!", true);
                    }
                }, null, () =>
                {
                    clicked = true;
                    playerController.walletController.DebitValue(MathDt.jailPrice);
                    playerController.LogMessagePlayer($"{playerController.name} pagou por um resgate urgente para sair da prisão!", false);
                    playerController.inJail = false;
                });
            }
        }

        yield return new WaitUntil(() => clicked == true);
    }

    public void ConfigFreeBoat(PlayerController playerController)
    {
        objFreeBoat.SetActive(true);
        Transform contentFreeBoat = objFreeBoat.transform.GetChild(0);
        contentFreeBoat.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        contentFreeBoat.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            clicked = true;
            playerController.inJail = false;
            playerController.freeBoat = false;
            playerController.LogMessagePlayer($"{playerController.name} utilizou um sinalizador para sair da prisão!", false);
            this.gameObject.SetActive(false);
        });
    }
}
