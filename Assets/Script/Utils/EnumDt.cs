using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumDt : MonoBehaviour
{
    enum tileType
    {
        property,
        jail,
        shares,
        card,
        start,
        gotoJail,
        luckyTry,
        none
    };

    public enum tradingBlock
    {
        sulamericano,
        americano,
        europeu,
        asiatico,
        africano,
        orientemedio,
        eslavo,
        oceanico
    };

    public enum storeProductType
    {
        dice,
        skin
    };

    public enum botBehavior
    {
        economic,
        normal,
        rich
    };
}


