using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int index;

    public virtual IEnumerator OnPlayerPass(PlayerController player)
    {
        yield return new WaitForSeconds(0.05f);
    }

    public virtual IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return null;
    }
}
