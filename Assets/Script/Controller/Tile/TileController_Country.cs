using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Country : TileController_Buyable
{
    public int level;

    private Transform buildingParent;

    public void Start()
    {
        buildingParent = this.transform.Find("Building");
    }

    public void UpgradeLevel(int level)
    {
        this.level = level;

        SetupBuilding();
    }

    public void SetupBuilding()
    {
        foreach(Transform aux in buildingParent)
        {
            aux.gameObject.SetActive(false);
        }

        if(Owner != null)
            buildingParent.GetChild(level).gameObject.SetActive(true);
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
            }
            else
            {
                ChangeTileColor(aux.gameObject, owner, baseColor);
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
}
