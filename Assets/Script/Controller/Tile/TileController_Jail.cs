using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Jail : TileController
{
    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        player.GotoJail();
        yield return null;
    }

    public override IEnumerator OnPlayerPass(PlayerController player)
    {
        //base.OnPlayerPass(player);

        yield return player.TurnCorner();
    }
}
