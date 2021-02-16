using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameManager manager;

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

    public void ResetAction()
    {
        foreach (var aux in tileControllers)
        {
            aux.onClickAction = null;
        }
    }

    public void SetupTravelBoard(PlayerController player)
    {
        //BOT
        if (!player.botController)
        {
            foreach (var aux in tileControllers)
            {
                aux.SetupTravel(player);
            }
        }
        else
        {
            List<TileTeleportBot> tileCanTeleport = new List<TileTeleportBot>();

            foreach(var aux in tileControllers)
            {
                if (aux.tileIsTeleported)
                {
                    int probability = 3;

                    if (aux.GetType() == typeof(TileController_Buyable))
                    {
                        var b = aux as TileController_Buyable;
                        if(b.Owner == null)
                        {
                            probability = 1;
                        }
                        else if(b.Owner == player)
                        {
                            probability = 2;
                        }
                        else
                        {
                            probability = 0;
                        }
                    }

                    tileCanTeleport.Add(new TileTeleportBot()
                    {
                        tile = aux,
                        probability = probability
                    });
                }
            }

            player.TravelPlayer(GetBestTileBot(tileCanTeleport));
        }
    }

    public TileController GetBestTileBot(List<TileTeleportBot> tileCanTeleport)
    {
        TileController bestTile;

        int randomProbability = UnityEngine.Random.Range(0, 100);
        int probability = 1;

        if(randomProbability<10)
        {
            probability = 3;
        }
        else if(randomProbability<30)
        {
            probability = 2;
        }

        var filtredList = tileCanTeleport.FindAll(n => n.probability == probability);

        bestTile = filtredList[UnityEngine.Random.Range(0,filtredList.Count)].tile ;

        return bestTile;
    }

    public void SetupMortgageBoard(PlayerController player)
    {
        foreach (var aux in tileControllers)
        {
            aux.SetupMortgage(player);
        }
    }

    public void ResetBoard()
    {
        foreach (var aux in tileControllers)
        {
            aux.ResetTile();
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
                        while(aux.Find(temp.name))
                        {
                            DestroyImmediate(aux.Find(temp.name).gameObject);
                        }
                        GameObject obj = Instantiate(temp, aux);
                        obj.transform.name = temp.name;
                    }

                    var tile = aux.GetComponent<TileController>().tile as TileBuyable_Country;
                    if (tile != null)
                    {
                        ColorSettings colorSettings = settingsManager.colorSettings;
                        Color backcolor = colorSettings.tradingBlockColor.Find(n => n.tradingBlock == tile.tradingBlock).color;
                        
                        Material[] mtList = aux.Find("Base").GetComponent<MeshRenderer>().sharedMaterials;
                        List<Material> newList = new List<Material>();

                        foreach(var auxMaterial in mtList)
                        {
                            if (auxMaterial.name == "TileColor")
                            {
                                var mat = new Material(auxMaterial);
                                mat.color = backcolor;
                                newList.Add(mat);
                            }
                            else
                            {
                                newList.Add(auxMaterial);
                            }
                        }

                        aux.Find("Base").GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();

                        //aux.GetComponent<SpriteRenderer>().color = backcolor;
                        aux.Find("Price").GetComponent<TextMesh>().text = MathDt.ConfigureMoney((int)tile.price);
                        aux.Find("CountryFlag").GetComponent<SpriteRenderer>().sprite = tile.flag;
                    }

                }
                if (aux.name.EndsWith("Wonder"))
                {
                    foreach (var temp in wonderChildPrefab)
                    {
                        while(aux.Find(temp.name))
                        {
                            DestroyImmediate(aux.Find(temp.name).gameObject);
                        }
                        GameObject obj = Instantiate(temp, aux);
                        obj.transform.name = temp.name;
                    }

                    var tile = aux.GetComponent<TileController>().tile as TileBuyable_Wonder;
                    if (tile != null)
                    {
                        ColorSettings colorSettings = settingsManager.colorSettings;

                        Color backcolor = colorSettings.wonderBackColor;

                        Material[] mtList = aux.Find("Base").GetComponent<MeshRenderer>().sharedMaterials;
                        List<Material> newList = new List<Material>();

                        foreach (var auxMaterial in mtList)
                        {
                            if (auxMaterial.name == "TileColor")
                            {
                                auxMaterial.color = backcolor;
                            }
                            newList.Add(auxMaterial);
                        }

                        aux.Find("Base").GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();

                        //aux.GetComponent<SpriteRenderer>().color = backcolor;
                        aux.Find("Price").GetComponent<TextMesh>().text = MathDt.ConfigureMoney((int)tile.price);
                    }

                }
            }
        }
    }
}
