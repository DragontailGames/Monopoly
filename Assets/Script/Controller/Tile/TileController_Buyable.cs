using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileController_Buyable : TileController
{
    private PlayerController owner;

    public CanvasManager canvas;

    public PlayerController Owner { get => this.owner; set => this.owner = value; }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return canvas.buyableMenu.SetupMenu(this, player);
    }

    public void BuyTile(PlayerController owner, string text, bool hostile)
    {
        owner.photonView.RPC("BuyTile_CMD", RpcTarget.All, this.index, text, hostile);
    }

    public virtual void OnBuy(PlayerController owner)
    {

    }
}
