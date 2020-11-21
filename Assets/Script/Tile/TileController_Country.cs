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

        buildingParent.GetChild(level - 1).gameObject.SetActive(true);
    }
}
