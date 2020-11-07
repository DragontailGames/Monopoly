using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Start : TileController
{
    public override IEnumerator OnPlayerPass(PlayerController player)
    {
       // base.OnPlayerPass(player);
        yield return player.TurnCorner();
    }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        base.OnPlayerStop(player);
        yield return null;
    }
}
