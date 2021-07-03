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
            if(player.playerNumber==manager.players.Count)
            {
                manager.NextPlayer();
            }
            player.DeclareBankruptcy();

            StopAllCoroutines();
            manager.ResetTransparentMaterial();
            StartCoroutine(manager.StartRound());
            //player.photonView.RPC("NextPlayer_CMD", Photon.Pun.RpcTarget.All);
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
