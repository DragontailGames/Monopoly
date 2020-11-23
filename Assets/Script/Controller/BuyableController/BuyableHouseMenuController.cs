﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableHouseMenuController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buyablePanel = new List<GameObject>();

    public List<string> constructionName;

    public List<Sprite> iconHouse;

    bool clicked = false;

    public IEnumerator SetupUpgradeTile(TileController_Country tile, PlayerController player)
    {
        this.gameObject.SetActive(true);

        clicked = false;

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
            icon.GetComponent<Image>().sprite = iconHouse[i];
            rentRate.GetComponent<TextMeshProUGUI>().text = "Rent rate: <b>$" + Math.ConfigureMoney((int)Math.GetRentPrice(fullPrice, i)) + "</b>";
            buy.GetComponentInChildren<TextMeshProUGUI>().text = "COMPRAR POR\n<size=32>$" + Math.ConfigureMoney(fullPrice) + "</size>";

            Button buyButton = buy.GetComponentInChildren<Button>();

            int level = i;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                clicked = true;
                player.DebitValue(fullPrice);
                tile.BuyTile(player);
                tile.UpgradeLevel(level);

                this.gameObject.SetActive(false);
            });
        }
        yield return new WaitUntil(() => clicked == true);
    }

}