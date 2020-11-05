using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int position = 0;

    public int ThrowDice()
    {
        return Random.Range(0,6);
    }

    public void MovePlayer()
    {
        int dice1 = ThrowDice();
        int dice2 = ThrowDice();

        Debug.Log("Dice value 1: " + dice1 + " - 2: " + dice2);


    }

    public void TurnCorner()
    {
        var newRot = this.transform.localRotation;
        newRot.y += 90;
        this.transform.localRotation = newRot;
    }
}