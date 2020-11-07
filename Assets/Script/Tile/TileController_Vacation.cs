using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Vacation : TileController
{
    public override IEnumerator OnPlayerPass(PlayerController player)
    {
        //base.OnPlayerPass(player);

        yield return player.TurnCorner();
    }
}
