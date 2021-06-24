using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController_Wonders : TileController_Buyable
{
    public void Update()
    {
        this.transform.Find("BaseFlag").gameObject.SetActive(Owner != null);
    }

    public override void OnBuy(PlayerController owner)
    {
        base.OnBuy(owner);

        var newMat = new Material(this.transform.Find("BaseFlag").GetComponent<MeshRenderer>().sharedMaterials[0]);
        newMat.color = owner.mainColor;
        List<Material> mats = new List<Material>() { newMat };
        this.transform.Find("BaseFlag").GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
    }
}
