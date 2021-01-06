using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Color Settings", menuName = "Monopoly/Color Settings", order = 7)]
public class ColorSettings : ScriptableObject
{
    public List<ColorByTradingBlock> tradingBlockColor = new List<ColorByTradingBlock>();

    public Color wonderBackColor;
}

[System.Serializable]
public class ColorByTradingBlock
{
    public EnumDt.tradingBlock tradingBlock = EnumDt.tradingBlock.sulamericano;
    public Color color;
}
