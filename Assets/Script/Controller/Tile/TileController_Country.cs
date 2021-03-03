using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Country : TileController_Buyable
{
    public int level;

    private Transform buildingParent;

    public int multiplier = 100;

    public int roundsWithMultiplier = 0;

    public void Start()
    {
        buildingParent = this.transform.Find("Building");
    }

    public void UpgradeLevel(int level, PlayerController player)
    {
        player.photonView.RPC("UpgradeLevel_CMD", Photon.Pun.RpcTarget.All, level, this.index);
    }

    public void SetupBuilding()
    {
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
    }

    private Color baseColor;

    public override void OnBuy(PlayerController owner)
    {
        base.OnBuy(owner);
        foreach (Transform aux in buildingParent)
        {
            if (owner != null)
            {
                ChangeTileColor(aux.gameObject,owner, owner.mainColor);
                transform.Find("CountryFlag").gameObject.SetActive(false);
            }
            else
            {
                ChangeTileColor(aux.gameObject, owner, baseColor);
                transform.Find("CountryFlag").gameObject.SetActive(true);
            }
        }
    }

    public void ChangeTileColor(GameObject obj, PlayerController owner, Color color)
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
}
