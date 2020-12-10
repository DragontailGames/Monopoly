using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Buyable : TileController
{
    private PlayerController owner;

    public CanvasManager canvas;

    public PlayerController Owner { get => this.owner; set { this.owner = value; OnBuy(this.owner); } }

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return canvas.buyableMenu.SetupMenu(this, player);
    }

    public void BuyTile(PlayerController owner)
    {
        this.Owner = owner;
        owner.properties.Add(this);

        owner.CheckWin();
        OnBuy(owner);
    }

    public virtual void OnBuy(PlayerController owner)
    {

    }
}
