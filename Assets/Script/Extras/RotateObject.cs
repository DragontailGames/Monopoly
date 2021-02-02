using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 10.0f;

    void LateUpdate()
    {
        this.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
