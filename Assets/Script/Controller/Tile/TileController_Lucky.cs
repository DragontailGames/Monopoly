using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Lucky : TileController
{
    public LuckyEffectManager luckyEffect;

    public LuckyMenuController luckyMenuController;

    public override IEnumerator OnPlayerStop(PlayerController player)
    {
        if(player.player.IsLocal)
        {
            TileLucky lucky = GetLuckyCard();
            yield return luckyMenuController.LuckyStart(lucky, player);
            yield return luckyEffect.StartLucky(player, lucky);
        }
    }

    public TileLucky GetLuckyCard()
    {
        var luckyObject = Resources.LoadAll("LuckyTile");
        List<TileLucky> tileLucky = new List<TileLucky>();
        foreach(var aux in luckyObject)
        {
            tileLucky.Add(aux as TileLucky);
        }

        return tileLucky[Random.Range(0, tileLucky.Count)];
    }
}
