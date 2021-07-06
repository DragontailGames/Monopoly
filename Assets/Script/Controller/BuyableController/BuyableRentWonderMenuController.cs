using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableRentWonderMenuController : MonoBehaviour
{
    bool clicked = false;

    public GameObject rentPanel;

    public GameObject hostileTakeoverPanel;

    public IEnumerator SetupRentWonderTile(TileController_Wonders tile, PlayerController player)
    {
        if(!player.botController)
            this.gameObject.SetActive(true);

        clicked = false;

        var tileWonder = tile.tile as TileBuyable_Wonder;

        int price = (int)MathDt.GetWonderRentPrice(tile.Owner.wondersInControl);

        Transform payRent = rentPanel.transform.GetChild(0).Find("Pay");

        payRent.GetComponentInChildren<TextMeshProUGUI>().text = "Pagar aluguel de $" + MathDt.ConfigureMoney(price);

        Button rentButton = payRent.GetComponentInChildren<Button>();

        var backgroundHeader = this.transform.Find("BackgroundHeader");
        backgroundHeader.GetComponent<Image>().color = tile.Owner.mainColor;
        backgroundHeader.GetChild(0).GetComponent<TextMeshProUGUI>().text = tile.tile.nameTile;

        rentButton.onClick.RemoveAllListeners();
        rentButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.walletController.DebitValue(price);
            tile.Owner.walletController.CreditValue(price);

            this.gameObject.SetActive(false);
        });

        int hostilePrice = MathDt.hostileWonderTakeoverPrice;

        Transform payHostile = hostileTakeoverPanel.transform.GetChild(0).Find("Buy");

        payHostile.GetComponentInChildren<TextMeshProUGUI>().text = "Compra hostil por $" + MathDt.ConfigureMoney(hostilePrice);

        Button hostileButton = payHostile.GetComponentInChildren<Button>();

        hostileButton.onClick.RemoveAllListeners();
        hostileButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.walletController.DebitValue(hostilePrice);
            tile.Owner.walletController.CreditValue(MathDt.wonderPrice);

            tile.BuyTile(player, $"{player.name} realizou uma aquisição hostil em: {tile.tile.nameTile}. Sendo {MathDt.wonderPrice} para {tile.Owner.name} e {hostilePrice} em impostos. e agora possui {player.wondersInControl + 1} maravilhas!", true);

            tile.Owner = player;

            this.gameObject.SetActive(false);
        });

        if (player.walletController.currentMoney <= hostilePrice)
            hostileButton.interactable = false;
        else
            hostileButton.interactable = true;

        //BOT
        if(player.botController)
        {
            yield return player.botController.ExecuteAction(()=> 
            {
                clicked = true;
                player.walletController.DebitValue(price);
                tile.Owner.walletController.CreditValue(price);
            },null,()=> 
            {
                clicked = true;
                player.walletController.DebitValue(hostilePrice);
                tile.Owner.walletController.CreditValue(MathDt.wonderPrice);
                tile.Owner = player;
            });
        }

        yield return new WaitUntil(() => clicked == true);
    }

    public void CloseButton()
    {
        clicked = true;
        this.gameObject.SetActive(false);
    }
}
