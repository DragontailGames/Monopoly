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
        this.gameObject.SetActive(true);

        clicked = false;

        var tileWonder = tile.tile as TileBuyable_Wonder;

        var content = wonderPanel.transform.GetChild(0);

        int price = (int)Math.GetWonderPrice();

        Transform title = content.transform.Find("Title");
        Transform icon = content.transform.Find("Icon");
        Transform buy = content.transform.Find("Buy");

        title.GetComponent<TextMeshProUGUI>().text = tileWonder.nameTile;
        icon.GetComponent<Image>().sprite = tileWonder.icon;
        buy.GetComponentInChildren<TextMeshProUGUI>().text = "COMPRAR POR\n<size=32>$" + Math.ConfigureMoney(price) + "</size>";

        Button buyButton = buy.GetComponent<Button>();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.DebitValue(price);
            tile.BuyTile(player);
            player.wondersInControl++;

            this.gameObject.SetActive(false);
        });

        yield return new WaitUntil(() => clicked == true);
    }
}
