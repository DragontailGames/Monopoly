using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Buyable : TileController
{
    public PlayerController owner;

    public CanvasManager canvas;

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return canvas.buyableMenu.SetupMenu(this, player);
    }

    public void BuyTile(PlayerController owner)
    {
        this.owner = owner;
    }
}
