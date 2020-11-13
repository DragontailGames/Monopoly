using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Tile Buyable", menuName = "Monopoly/Tile Buyable", order = 1)]
public class TileBuyable : Tile
{
    public float price;
}
