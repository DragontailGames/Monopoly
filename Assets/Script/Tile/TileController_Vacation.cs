using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Vacation : TileController
{
    public BoardController board;

    public override IEnumerator OnPlayerPass(PlayerController player)
    {
        //base.OnPlayerPass(player);

        yield return player.TurnCorner();
    }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        player.canTeleport = true;
        yield return null;
    }
}
