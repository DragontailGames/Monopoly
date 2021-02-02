using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Tile Country", menuName = "Monopoly/Tile Country", order = 2)]
public class TileBuyable_Country : TileBuyable
{
    public Sprite flag;

    public EnumDt.tradingBlock tradingBlock;

    public GameObject flagObject;
}
