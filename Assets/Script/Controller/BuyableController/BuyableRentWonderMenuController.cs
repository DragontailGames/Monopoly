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
        this.gameObject.SetActive(true);
        clicked = false;

        var tileWonder = tile.tile as TileBuyable_Wonder;

        int price = (int)Math.GetWonderRentPrice(tile.owner.wondersInControl);

        Transform payRent = rentPanel.transform.GetChild(0).Find("Pay");

        payRent.GetComponentInChildren<TextMeshProUGUI>().text = "Pagar aluguel de $" + Math.ConfigureMoney(price);

        Button rentButton = payRent.GetComponentInChildren<Button>();

        rentButton.onClick.RemoveAllListeners();
        rentButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.DebitValue(price);
            tile.owner.CreditValue(price);

            this.gameObject.SetActive(false);
        });

        int hostilePrice = Math.GetHostileWonderTakeoverPrice();

        Transform payHostile = hostileTakeoverPanel.transform.GetChild(0).Find("Buy");

        payHostile.GetComponentInChildren<TextMeshProUGUI>().text = "Compra hostil por $" + Math.ConfigureMoney(hostilePrice);

        Button hostileButton = payHostile.GetComponentInChildren<Button>();

        hostileButton.onClick.RemoveAllListeners();
        hostileButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.DebitValue(hostilePrice);
            tile.owner.CreditValue(Math.GetWonderPrice());
            tile.owner = player;

            this.gameObject.SetActive(false);
        });

        yield return new WaitUntil(() => clicked == true);
    }
}
