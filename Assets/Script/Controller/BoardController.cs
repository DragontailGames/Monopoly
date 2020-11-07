using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public List<TileController> tileControllers = new List<TileController>();

    public GameObject board;

    private void Start()
    {
        foreach(TileController aux in board.transform.GetComponentsInChildren<TileController>())
        {
                tileControllers.Add(aux);
        }
        tileControllers.Sort((a, b) => { return a.index.CompareTo(b.index); });
    }
}
