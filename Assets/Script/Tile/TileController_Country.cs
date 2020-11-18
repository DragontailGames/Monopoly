using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Country : TileController
{
    public CanvasManager canvas;

    public PlayerController owner;

    public int level;

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return canvas.buyableMenu.SetupMenu(this, player);
    }
}
