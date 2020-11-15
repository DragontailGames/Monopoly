using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public List<TileController> tileControllers = new List<TileController>();

    public GameObject board;

    private void Start()
    {
        foreach(TileController aux in board.transform.GetComponentsInChildren<TileController>())
        {
                tileControllers.Add(aux);
        }
        tileControllers.Sort((a, b) => { return a.index.CompareTo(b.index); });
    }

    [ContextMenu("Load county details")]
    public void LoadAllCountryDetails()
    {
        foreach(Transform aux in this.transform)
        {
            if(aux.GetComponent<TileController>())
            {
                var tile = aux.GetComponent<TileController>().tile as TileBuyable_Country;
                if (tile != null)
                {
                    Color backcolor = SettingsManager.instance.colorSettings.tradingBlockColor.Find(n => n.tradingBlock == tile.tradingBlock).color;
                    aux.GetComponent<SpriteRenderer>().color = backcolor;
                    aux.Find("Price").GetComponent<TextMesh>().text = ConfigureMoney((int)tile.price);
                    aux.Find("CountryFlag").GetComponent<SpriteRenderer>().sprite = tile.flag;
                }
            }
        }
    }

    public string ConfigureMoney(int money)
    {
        if (money > 100)
        {
            return string.Format($"{money / 1000:#,##0K}");
        }
        else
        {
            return money.ToString();
        }
    }
}
