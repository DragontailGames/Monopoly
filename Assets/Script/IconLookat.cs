using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconLookat : MonoBehaviour
{
    public void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-Vector3.one);
    }
}
