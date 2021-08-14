using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Country : TileController_Buyable
{
    public int level;

    private Transform buildingParent;

    private int multiplier = 100;

    public int roundsWithMultiplier = 0;

    private GameObject bonus, down;

    public int Multiplier { get => this.multiplier; }

    public void Start()
    {
        buildingParent = this.transform.Find("Building");

        bonus = Instantiate(boardController.bonus, this.gameObject.transform);
        down = Instantiate(boardController.down, this.gameObject.transform);
    }

    public void UpgradeLevel(int level, PlayerController player, bool hasOwner = true)
    {
        player.photonView.RPC("UpgradeLevel_CMD", Photon.Pun.RpcTarget.All, level, this.index, hasOwner);
    }

    public void SetupBuilding(bool hasOwner)
    {
        if(!hasOwner)
        {
            Owner = null;
        }

        foreach(Transform aux in buildingParent)
        {
            aux.gameObject.SetActive(false);
        }

        if (Owner != null)
        {
            var construction = buildingParent.GetChild(level);
            construction.gameObject.SetActive(true);

            var flag = construction.GetChild(0);
            TileBuyable_Country coutryTile = tile as TileBuyable_Country;

            var newFlag = Instantiate(coutryTile.flagObject,construction);
            newFlag.transform.position = flag.transform.position;
            newFlag.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 140));
            newFlag.transform.localScale = flag.transform.localScale;

            Destroy(flag.gameObject);
        }
        else
        {
            transform.Find("ExtraMaterials").gameObject.SetActive(true);
        }
    }

    private Color baseColor;

    public override void OnBuy(PlayerController owner)
    {
        base.OnBuy(owner);
        foreach (Transform aux in buildingParent)
        {
            if (owner != null)
            {
                ChangeTileColor(aux.gameObject, owner.mainColor);
                transform.Find("ExtraMaterials").gameObject.SetActive(false);
            }
            else
            {
                ChangeTileColor(aux.gameObject, baseColor);
                transform.Find("ExtraMaterials").gameObject.SetActive(true);
            }
        }
    }

    public void ChangeTileColor(GameObject obj, Color color)
    {
        var mtList = obj.GetComponent<MeshRenderer>().sharedMaterials;
        List<Material> newList = new List<Material>();

        foreach (var auxMaterial in mtList)
        {
            if (auxMaterial.name == "PlayerMaterial")
            {
                var mat = new Material(auxMaterial);
                baseColor = mat.color;
                mat.color = color;
                newList.Add(mat);
            }
            else
            {
                newList.Add(auxMaterial);
            }
        }

        obj.GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
        if(roundsWithMultiplier>0)
        {
            roundsWithMultiplier--;
        }
        if(roundsWithMultiplier<=0)
        {
            multiplier = 100;
        }
    }

    private Color basePriceColor;

    public void SetupMultiplier(int multiplier, PlayerController player)
    {
        if(player != null)
        {
            player.SetupTileMultipler(this.index, multiplier);
        }
        else
        {
            this.multiplier = multiplier;
            if (multiplier > 100)
            {
                bonus.SetActive(true);
                var mats = this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;
                List<Material> newList = new List<Material>();

                foreach (var auxMaterial in mats)
                {
                    if (auxMaterial.name == "PriceBase")
                    {
                        var mat = new Material(auxMaterial);
                        basePriceColor = mat.color;
                        mat.color = boardController.colorBonus;
                        newList.Add(mat);
                    }
                    else
                    {
                        newList.Add(auxMaterial);
                    }
                }

                this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
            }
            else if (multiplier < 100)
            {
                down.SetActive(true);
                var mats = this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;
                List<Material> newList = new List<Material>();

                foreach (var auxMaterial in mats)
                {
                    if (auxMaterial.name == "PriceBase")
                    {
                        var mat = new Material(auxMaterial);
                        basePriceColor = mat.color;
                        mat.color = boardController.colorDown;
                        newList.Add(mat);
                    }
                    else
                    {
                        newList.Add(auxMaterial);
                    }
                }

                this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
            }
            else
            {
                bonus.SetActive(false);
                down.SetActive(false);
                var mats = this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;
                List<Material> newList = new List<Material>();

                foreach (var auxMaterial in mats)
                {
                    if (auxMaterial.name == "PriceBase")
                    {
                        var mat = new Material(auxMaterial);
                        mat.color = basePriceColor;
                        newList.Add(mat);
                    }
                    else
                    {
                        newList.Add(auxMaterial);
                    }
                }

                this.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
            }
        }

    }
}
