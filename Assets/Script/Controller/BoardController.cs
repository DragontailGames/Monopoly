using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public List<TileController> tileControllers = new List<TileController>();

    public GameObject board;

    public List<GameObject> countryChildPrefab = new List<GameObject>();

    public List<GameObject> wonderChildPrefab = new List<GameObject>();

    public SettingsManager settingsManager;

    public TileController_Jail jail;

    private void Start()
    {
        foreach(TileController aux in board.transform.GetComponentsInChildren<TileController>())
        {
            tileControllers.Add(aux);
            aux.boardController = this;
        }
        tileControllers.Sort((a, b) => { return a.index.CompareTo(b.index); });
    }

    public void ResetPlayerToTeleport()
    {
        foreach (var aux in tileControllers)
        {
            aux.playerToTeleport = null;
        }
    }

    public void SetupTeleportBoard(PlayerController player)
    {
        foreach(var aux in tileControllers)
        {
            aux.SetupTeleport(player);
        }
    }

    [ContextMenu("Load county details")]
    public void LoadAllCountryDetails()
    { 
        foreach(Transform aux in this.transform)
        {
            
            if(aux.GetComponent<TileController>())
            {
                if (aux.name.EndsWith("Country"))
                {
                    foreach (var temp in countryChildPrefab)
                    {
                        if (!aux.Find(temp.name))
                        {
                            GameObject obj = Instantiate(temp, aux);
                            obj.transform.name = temp.name;
                        }
                    }

                    var tile = aux.GetComponent<TileController>().tile as TileBuyable_Country;
                    if (tile != null)
                    {
                        ColorSettings colorSettings = settingsManager.colorSettings;
                        Color backcolor = colorSettings.tradingBlockColor.Find(n => n.tradingBlock == tile.tradingBlock).color;
                        aux.GetComponent<SpriteRenderer>().color = backcolor;
                        aux.Find("Price").GetComponent<TextMesh>().text = Math.ConfigureMoney((int)tile.price);
                        aux.Find("CountryFlag").GetComponent<SpriteRenderer>().sprite = tile.flag;
                    }

                }
                if (aux.name.EndsWith("Wonder"))
                {
                    foreach (var temp in wonderChildPrefab)
                    {
                        if (!aux.Find(temp.name))
                        {
                            GameObject obj = Instantiate(temp, aux);
                            obj.transform.name = temp.name;
                        }
                    }

                    var tile = aux.GetComponent<TileController>().tile as TileBuyable_Wonder;
                    if (tile != null)
                    {
                        ColorSettings colorSettings = settingsManager.colorSettings;
                        Color backcolor = colorSettings.wonderBackColor;
                        aux.GetComponent<SpriteRenderer>().color = backcolor;
                        aux.Find("Price").GetComponent<TextMesh>().text = Math.ConfigureMoney((int)tile.price);
                        aux.Find("IconWonder").GetComponent<SpriteRenderer>().sprite = tile.icon;
                    }

                }
            }
        }
    }
}
