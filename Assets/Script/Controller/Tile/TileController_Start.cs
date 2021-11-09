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
            Debug.Log("Entrou no Player First Buy");
            var manager = player.manager;

          //  StopAllCoroutines();
            manager.ResetTransparentMaterial();
            player.DeclareBankruptcy();
            Debug.Log("Pedro " + manager.currentPlayer + " - " + manager.players.Count);
            player.photonView.RPC("NextPlayer_CMD", Photon.Pun.RpcTarget.All, manager.currentPlayer == manager.players.Count);
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
