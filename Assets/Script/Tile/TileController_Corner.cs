using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Corner : TileController
{
    public override void WhenPlayerStop(PlayerController player)
    {
        base.WhenPlayerStop(player);

        player.TurnCorner();
    }
}
