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

    public override void OnBuy(PlayerController owner)
    {
        base.OnBuy(owner);
        foreach (Transform aux in buildingParent)
        {
            if (owner != null)
            {
                aux.GetComponent<Outline>().enabled = true;
                aux.GetComponent<Outline>().OutlineColor = owner.mainColor;
            }
            else
            {
                aux.GetComponent<Outline>().enabled = false;
            }
        }
    }
}
