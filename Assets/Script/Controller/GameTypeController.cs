using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTypeController : MonoBehaviour
{
    public int gameSelected = 1;

    public void End()
    {
        this.transform.GetChild(gameSelected).gameObject.SetActive(true);
    }

    public void Begin()
    {
        foreach(Transform aux in this.transform)
        {
            aux.gameObject.SetActive(false);
        }
    }

}
