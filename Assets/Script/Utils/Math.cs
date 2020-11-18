using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{

    public static List<int> percentageOfPrice = new List<int>()
    {
        0,
        20,
        50,
        80
    };

    public static List<int> rentOfPrice = new List<int>()
    {
        10,
        20,
        30,
        50
    };

    public static float GetContructionPrice(float basePrice, int contructionLevel)
    {
        return basePrice * (1.0f + (float)percentageOfPrice[contructionLevel] / 100);
    }

    public static float GetRentPrice(float basePrice, int contructionLevel)
    {
        return basePrice * ((float)rentOfPrice[contructionLevel] / 100);
    }

    public static string ConfigureMoney(int money)
    {
        if (money > 100)
        {
            return string.Format($"{money / 1000:#,##0K}");
        }
        else
        {
            return money.ToString();
        }
    }
}
