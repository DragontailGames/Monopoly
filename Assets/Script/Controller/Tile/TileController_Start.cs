using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Start : TileController
{
    public int startMoney;

    public override IEnumerator OnPlayerPass(PlayerController player)
    {
        base.OnPlayerPass(player);

        if (!player.firstBuy)
        {
            var manager = player.manager;
            player.DeclareBankruptcy();

            StopAllCoroutines();
            manager.ResetTransparentMaterial();
            yield break;
        }

        if (!player.fakeTravel)
            player.walletController.CreditValue(startMoney);

        yield return player.TurnCorner();
    }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        base.OnPlayerStop(player);
        yield return null;
    }
}
