using System.Collections;
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
        var tileBuyable = tile.tile as TileBuyable_Country;
        int startValue = tile.level;

        if (player.botController)
        {
            //BOT
            yield return player.botController.ExecuteAction(() =>
            {
                int level = startValue + 1;

                int fullPrice = (int)MathDt.GetContructionPrice(tileBuyable.price, level, tile.level);

                clicked = true;
                player.walletController.DebitValue(fullPrice);
                tile.BuyTile(player);
                player.firstBuy = true;
                tile.UpgradeLevel(level, player);

            }, null, () =>
            {
                int randLevel = Random.Range(0,100);
                int level = 3;
                if (randLevel<30)
                    level = startValue + 1;
                else if (randLevel < 70)
                    level = startValue + 2;

                if(level>3)
                {
                    level = 3;
                }


                int fullPrice = (int)MathDt.GetContructionPrice(tileBuyable.price, level, tile.level);

                clicked = true;
                player.walletController.DebitValue(fullPrice);
                tile.BuyTile(player);
                player.firstBuy = true;
                tile.UpgradeLevel(level, player);
            });
        }
        else
        {
            this.gameObject.SetActive(true);

            clicked = false;

            if (tile.level < 4 && tile.Owner != null && tile.level > 0)
            {
                startValue++;
            }

            foreach (var aux in buyablePanel)
            {
                aux.SetActive(false);
            }

            for (int i = startValue; i < 4; i++)
            {
                buyablePanel[i].SetActive(true);
                var content = buyablePanel[i].transform.GetChild(0);

                int fullPrice = (int)MathDt.GetContructionPrice(tileBuyable.price, i, tile.level);

                Transform title = content.transform.Find("Title");
                Transform icon = content.transform.Find("Icon");
                Transform rentRate = content.transform.Find("Rent Rate");
                Transform buy = content.transform.Find("Buy");

                title.GetComponent<TextMeshProUGUI>().text = constructionName[i];
                icon.GetComponent<Image>().sprite = iconHouse[i];
                rentRate.GetComponent<TextMeshProUGUI>().text = "Rent rate: <b>$" + MathDt.ConfigureMoney((int)MathDt.GetRentPrice(tileBuyable.price, i)) + "</b>";
                buy.GetComponentInChildren<TextMeshProUGUI>().text = "COMPRAR POR\n<size=32>$" + MathDt.ConfigureMoney(fullPrice) + "</size>";

                Button buyButton = buy.GetComponentInChildren<Button>();

                int level = i;

                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    clicked = true;
                    player.walletController.DebitValue(fullPrice);
                    tile.BuyTile(player);
                    player.firstBuy = true;
                    tile.UpgradeLevel(level, player);

                    this.gameObject.SetActive(false);
                });

                if (player.walletController.currentMoney <= fullPrice)
                    buyButton.interactable = false;
                else
                    buyButton.interactable = true;
            }
        }
        yield return new WaitUntil(() => clicked == true);
    }

    public void CloseButton()
    {
        clicked = true;
        this.gameObject.SetActive(false);
    }

}
