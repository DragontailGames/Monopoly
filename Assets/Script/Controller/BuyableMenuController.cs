using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject focusPanel;

    [SerializeField]
    private List<GameObject> buyablePanel = new List<GameObject>();

    [SerializeField]
    private List<string> constructionName;

    [SerializeField]
    private List<Sprite> icon = new List<Sprite>();

    public IEnumerator SetupMenu(TileController_Country tile, PlayerController player)
    {
        if(tile.owner == player || tile.owner == null)
        {
            yield return SetupUpgradeTile(tile, player);
        }
    }

    private IEnumerator SetupUpgradeTile(TileController_Country tile, PlayerController player)
    {
        focusPanel.SetActive(true);

        for (int i = tile.level;i<4;i++)
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

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                clicked = true;
                player.DebitValue(fullPrice);

                focusPanel.SetActive(false);
            });
        }
        yield return new WaitUntil(() => WaitForClick());
    }

    bool clicked = false;

    public bool WaitForClick()
    {
        return clicked;
    }
}
