using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Product", menuName = "Product/Product", order = 0)]
public class Product : ScriptableObject
{
    public string id;

    public int diamondCost;

    public string productName;

    public EnumDt.storeProductType productType;

    public Sprite icon;
}
