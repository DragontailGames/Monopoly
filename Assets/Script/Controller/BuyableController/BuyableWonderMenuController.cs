using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableWonderMenuController : MonoBehaviour
{
    bool clicked = false;

    public GameObject wonderPanel;

    public IEnumerator SetupWonderTile(TileController_Wonders tile, PlayerController player)
    {
        if(!player.botController)
            this.gameObject.SetActive(true);

        clicked = false;

        var backgroundHeader = this.transform.Find("BackgroundHeader");
        backgroundHeader.GetChild(0).GetComponent<TextMeshProUGUI>().text = tile.tile.nameTile;

        var tileWonder = tile.tile as TileBuyable_Wonder;

        var content = wonderPanel.transform.GetChild(0);

        int price = MathDt.wonderPrice;

        Transform title = content.transform.Find("Title");
        Transform icon = content.transform.Find("Icon");
        Transform buy = content.transform.Find("Buy");

        title.GetComponent<TextMeshProUGUI>().text = tileWonder.nameTile;
        icon.GetComponent<Image>().sprite = tileWonder.icon;
        buy.GetComponentInChildren<TextMeshProUGUI>().text = "COMPRAR POR\n<size=32>$" + MathDt.ConfigureMoney(price) + "</size>";

        Button buyButton = buy.GetComponent<Button>();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.walletController.DebitValue(price);
            tile.BuyTile(player, $"{tile.tile.nameTile} e agora possui {player.wondersInControl + 1} maravilhas!", false);//MESSAGE WONDER
            player.firstBuy = true;
            player.wondersInControl++;

            player.WonderWin();

            this.gameObject.SetActive(false);
        });

        if (player.walletController.currentMoney <= price)
            buyButton.interactable = false;
        else
            buyButton.interactable = true;

        //BOT
        if (player.botController)
        {
            yield return player.botController.ExecuteAction(() =>
            {
                clicked = true;
                player.walletController.DebitValue(price);
                tile.BuyTile(player, $"{tile.tile.nameTile} e agora possui {player.wondersInControl + 1} maravilhas!", false);//MESSAGE WONDER
                player.firstBuy = true;
                player.wondersInControl++;

                player.WonderWin();
            }, () => { });
        }

        yield return new WaitUntil(() => clicked == true);
    }

    public void CloseButton()
    {
        clicked = true;
        this.gameObject.SetActive(false);
    }
}
