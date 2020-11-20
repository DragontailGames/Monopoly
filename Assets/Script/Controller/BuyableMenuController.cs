using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject focusPanel;

    [Header("Buyable")]

    [SerializeField]
    private GameObject buyableMenu;

    [SerializeField]
    private List<GameObject> buyablePanel = new List<GameObject>();

    [Header("Rent")]

    [SerializeField]
    private GameObject rentMenu;

    [SerializeField]
    private GameObject rentPanel;

    [SerializeField]
    private GameObject hostileTakeoverPanel;

    [SerializeField]
    private List<string> constructionName;

    [SerializeField]
    private List<Sprite> icon = new List<Sprite>();

    bool clicked = false;

    public IEnumerator SetupMenu(TileController_Country tile, PlayerController player)
    {
        focusPanel.SetActive(true);
        if (tile.owner == player || tile.owner == null)
        {
            yield return SetupUpgradeTile(tile, player);
        }
        else
        {
            yield return SetupRentTile(tile, player);
        }
    }

    private IEnumerator SetupUpgradeTile(TileController_Country tile, PlayerController player)
    {
        buyableMenu.SetActive(true);
        for (int i = tile.level; i < 4; i++)
        {
            var tileBuyable = tile.tile as TileBuyable_Country;
            var content = buyablePanel[i].transform.GetChild(0);

            int fullPrice = (int)Math.GetContructionPrice(tileBuyable.price, i);

            Transform title = content.transform.Find("Title");
            Transform icon = content.transform.Find("Icon");
            Transform rentRate = content.transform.Find("Taxa de Aluguel");
            Transform buy = content.transform.Find("Buy");

            title.GetComponent<TextMeshProUGUI>().text = constructionName[i];
            icon.GetComponent<Image>().sprite = this.icon[i];
            rentRate.GetComponent<TextMeshProUGUI>().text = "Rent rate: <b>$" + Math.ConfigureMoney((int)Math.GetRentPrice(fullPrice, i)) + "</b>";
            buy.GetComponentInChildren<TextMeshProUGUI>().text = "COMPRAR POR\n<size=32>$" + Math.ConfigureMoney(fullPrice) + "</size>";

            Button buyButton = buy.GetComponentInChildren<Button>();

            int level = i;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                clicked = true;
                player.DebitValue(fullPrice);
                tile.BuyTile(player, level);

                focusPanel.SetActive(false);

                buyableMenu.SetActive(false);
            });
        }
        yield return new WaitUntil(() => clicked == true);
    }

    private IEnumerator SetupRentTile(TileController_Country tile, PlayerController player)
    {
        rentMenu.SetActive(true);

        var tileBuyable = tile.tile as TileBuyable_Country;

        int rentPrice = (int)Math.GetRentPrice(tileBuyable.price, tile.level);

        Transform payRent = rentPanel.transform.GetChild(0).Find("Pay");
        payRent.GetComponentInChildren<TextMeshProUGUI>().text = "Pagar aluguel de $" + Math.ConfigureMoney(rentPrice);

        Button rentButton = payRent.GetComponentInChildren<Button>();

        rentButton.onClick.RemoveAllListeners();
        rentButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.DebitValue(rentPrice);
            tile.owner.CreditValue(rentPrice);

            focusPanel.SetActive(false);

            buyableMenu.SetActive(false);
        });

        int hostilePrice = (int)Math.GetHostileTakeoverPrice((int)Math.GetContructionPrice(tileBuyable.price, tile.level));

        Transform payHostile = hostileTakeoverPanel.transform.GetChild(0).Find("Buy");
        payHostile.GetComponentInChildren<TextMeshProUGUI>().text = "Compra hostil por $" + Math.ConfigureMoney(rentPrice);

        Button hostileButton = payHostile.GetComponentInChildren<Button>();

        hostileButton.onClick.RemoveAllListeners();
        hostileButton.onClick.AddListener(() =>
        {
            clicked = true;
            player.DebitValue(hostilePrice);
            tile.owner.CreditValue((int)Math.GetContructionPrice(tileBuyable.price, tile.level));
            tile.owner = player;

            focusPanel.SetActive(false);

            buyableMenu.SetActive(false);
        });

        yield return new WaitUntil(() => clicked == true);
    }
}
