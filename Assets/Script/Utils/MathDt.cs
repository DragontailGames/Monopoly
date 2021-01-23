using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathDt
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

    public static int jailPrice = 200000;

    public static int wonderPrice = 200000;

    public static int hostileWonderTakeoverPrice = 1000000;

    public static float GetContructionPrice(float basePrice, int contructionLevel, int currentLevel)
    {
        float constructionPrice = basePrice * (1.0f + (float)percentageOfPrice[contructionLevel] / 100);

        if(currentLevel>0)
        {
            constructionPrice -= basePrice * (1.0f + (float)percentageOfPrice[currentLevel] / 100);
        }

        return constructionPrice;
    }
    public static float GetContructionPrice(float basePrice, int contructionLevel)
    {
        float constructionPrice = basePrice * (1.0f + (float)percentageOfPrice[contructionLevel] / 100);

        return constructionPrice;
    }

    public static float GetRentPrice(float basePrice, int contructionLevel)
    {
        return basePrice * ((float)rentOfPrice[contructionLevel] / 100);
    }

    public static float GetHostileTakeoverPrice(float price)
    {
        return price * 2;
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

    public static int GetWonderRentPrice(int wonders)
    {
        return 50000 * wonders;
    }

    internal static int GetMortgagePrice(TileController_Buyable btile)
    {
        float percentage = 0.75f;
        TileBuyable b = btile.tile as TileBuyable;

        if(btile.GetType() == typeof(TileController_Country))
        {
            TileController_Country country = btile as TileController_Country;
            return (int)(GetContructionPrice(b.price, country.level) * percentage);
        }
        else
        {
            return (int)(wonderPrice * percentage);
        }

    }

    public static bool IsBetween(double testValue, double bound1, double bound2)
    {
        return (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
    }
}
