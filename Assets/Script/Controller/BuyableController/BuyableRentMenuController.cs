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
        this.gameObject.SetActive(true);

        clicked = false;

        var tileBuyable = tile.tile as TileBuyable_Country;

        int rentPrice = (int)Math.GetRentPrice(tileBuyable.price, tile.level);

        Transform payRent = rentPanel.transform.GetChild(0).Find("Pay");
        payRent.GetComponentInChildren<TextMeshProUGUI>().text = "Pagar aluguel de $" + Math.ConfigureMoney(rentPrice);

        Button rentButton = payRent.GetComponentInChildren<Button>();

        rentButton.onClick.RemoveAllListeners();
        rentButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.TransferMoney(rentPrice, rentPrice, tile.owner);
            this.gameObject.SetActive(false);
        });

        int hostilePrice = (int)Math.GetHostileTakeoverPrice((int)Math.GetContructionPrice(tileBuyable.price, tile.level, tile.level));

        Transform payHostile = hostileTakeoverPanel.transform.GetChild(0).Find("Buy");
        payHostile.GetComponentInChildren<TextMeshProUGUI>().text = "Compra hostil por $" + Math.ConfigureMoney(rentPrice);

        Button hostileButton = payHostile.GetComponentInChildren<Button>();

        hostileButton.onClick.RemoveAllListeners();
        hostileButton.onClick.AddListener(() =>
        {
            clicked = true;
            int creditValue = (int)Math.GetContructionPrice(tileBuyable.price, tile.level, tile.level);
            player.TransferMoney(hostilePrice, creditValue, tile.owner);
            tile.owner = player;

            this.gameObject.SetActive(false);
        });

        if (player.currentMoney <= hostilePrice)
            hostileButton.interactable = false;
        else
            hostileButton.interactable = true;

        yield return new WaitUntil(() => clicked == true);
    }

    public void CloseButton()
    {
        clicked = true;
    }
}
