using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dragontailgames.Utils;

public class Names : MonoBehaviour
{
    [ContextMenu("Save Names")]
    public void SaveNames()
    {
        RandomStuffs rand = new RandomStuffs()
        {
            names = new List<string>()
            {
                "PedrinhoJogador",
                "monitorgrimacing",
                "steercombative",
                "hidemeaning",
                "timberheadview",
                "lutestringbefore",
                "scorebench",
                "wordyspring",
                "brokencalling",
                "pupposse",
                "vanishingconsonant",
                "harasssiege",
                "saidforestay",
                "stradabsolutely",
                "patternfeeling",
                "processionaryhorror",
                "caughtsweeping",
                "unrulyhundred",
                "bankcloistered",
                "handprominent"
            }
        };

        SaveAndLoad.Save<RandomStuffs>("Names", rand);
    }

    public static string GetName()
    {
        var names = (SaveAndLoad.DeserializeObject<RandomStuffs>(SaveAndLoad.Load("Names"))).names;
        return names[Random.Range(0,names.Count)];
    }
}
