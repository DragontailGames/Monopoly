using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Tile Lucky", menuName = "Monopoly/Tile Lucky", order = 4)]
public class TileLucky : ScriptableObject
{
    public string text;

    public string method;

    public int value;

    public int percentage;

    public EnumDt.luckType luckType;

    public int tileIndex;

    public string tileName;

    public bool ownerTileEffect;

    public bool vacation = false;
}
