using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BabelDt
{
    public static string terrain_br = "Terreno";
    public static string house_br = "Casa";
    public static string mansion_br = "Mansao";
    public static string hotel_br = "Hotel";

    public static string TileLevelName(int level)
    {
        if (level == 0)
            return terrain_br;
        if (level == 1)
            return house_br;
        if (level == 2)
            return mansion_br;
        if (level == 3)
            return hotel_br;
        return "Wrong Level";
    }
}
