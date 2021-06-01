using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableRentMenuController : MonoBehaviour
{
    public GameObject rentPanel;

    public GameObject hostileTakeoverPanel;

    bool clicked = false;


    public IEnumerator SetupRentTile(TileController_Country tile, PlayerController player)
    {
        if (!player.botController)
        {
            this.gameObject.SetActive(true);
        }

        clicked = false;

        var tileBuyable = tile.tile as TileBuyable_Country;

        int rentPrice = (int)MathDt.GetRentPrice(tileBuyable.price, tile.level) * (tile.multiplier/100);

        Transform payRent = rentPanel.transform.GetChild(0).Find("Pay");
        payRent.GetComponentInChildren<TextMeshProUGUI>().text = "Pagar aluguel de $" + MathDt.ConfigureMoney(rentPrice);

        Button rentButton = payRent.GetComponentInChildren<Button>();

        rentButton.onClick.RemoveAllListeners();
        rentButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.walletController.TransferMoney(rentPrice, rentPrice, tile.Owner);
            player.LogMessagePlayer($"{player.name} pagou {rentPrice} para {tile.Owner} em: {tile.tile.nameTile}", true);
            this.gameObject.SetActive(false);
        });

        int hostilePrice = (int)MathDt.GetHostileTakeoverPrice((int)MathDt.GetContructionPrice(tileBuyable.price, tile.level));

        Transform payHostile = hostileTakeoverPanel.transform.GetChild(0).Find("Buy");
        payHostile.GetComponentInChildren<TextMeshProUGUI>().text = "Compra hostil por $" + MathDt.ConfigureMoney(hostilePrice);

        Button hostileButton = payHostile.GetComponentInChildren<Button>();

        hostileButton.onClick.RemoveAllListeners();
        hostileButton.onClick.AddListener(() =>
        {
            clicked = true;
            int creditValue = (int)MathDt.GetContructionPrice(tileBuyable.price, tile.level);
            player.walletController.TransferMoney(hostilePrice, creditValue, tile.Owner);
            tile.Owner.properties.Remove(tile.Owner.properties.Find(n => n.index == tile.index));
            player.LogMessagePlayer($"{player.name} realizou uma aquisição hostil em: {tileBuyable.nameTile}. Sendo {tileBuyable.price} para {tile.Owner.name} e {tileBuyable.price} em impostos.", true);
            tile.Owner = player;
            tile.Owner.properties.Add(tile);

            this.gameObject.SetActive(false);
        });

        if (player.walletController.currentMoney <= hostilePrice)
            hostileButton.interactable = false;
        else
            hostileButton.interactable = true;

        //BOT
        if (player.botController)
        {
            yield return player.botController.ExecuteAction(() =>
            {
                clicked = true;
                player.walletController.TransferMoney(rentPrice, rentPrice, tile.Owner);
            }, null, () =>
            {
                clicked = true;
                int creditValue = (int)MathDt.GetContructionPrice(tileBuyable.price, tile.level);
                player.walletController.TransferMoney(hostilePrice, creditValue, tile.Owner);
                tile.Owner.properties.Remove(tile.Owner.properties.Find(n => n.index == tile.index));
                player.LogMessagePlayer($"{player.name} realizou uma aquisição hostil em: {tileBuyable.nameTile}. Sendo {creditValue} para {tile.Owner.name} e {creditValue} em impostos.", true);
                tile.Owner = player;
                tile.Owner.properties.Add(tile);
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
