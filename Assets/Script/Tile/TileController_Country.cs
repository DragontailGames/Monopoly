using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Country : TileController
{
    public CanvasManager canvas;

    public PlayerController owner;

    public int level;

    private Transform buildingParent;

    public void Start()
    {
        buildingParent = this.transform.Find("Building");
    }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return canvas.buyableMenu.SetupMenu(this, player);
    }

    public void BuyTile(PlayerController owner, int level)
    {
        this.owner = owner;
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
