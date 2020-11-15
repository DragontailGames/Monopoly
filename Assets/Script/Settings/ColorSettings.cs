using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Color Settings", menuName = "Monopoly/Color Settings", order = 4)]
public class ColorSettings : ScriptableObject
{
    public List<ColorByTradingBlock> tradingBlockColor = new List<ColorByTradingBlock>();
}

[System.Serializable]
public class ColorByTradingBlock
{
    public Enum.tradingBlock tradingBlock = Enum.tradingBlock.sulamericano;
    public Color color;
}
