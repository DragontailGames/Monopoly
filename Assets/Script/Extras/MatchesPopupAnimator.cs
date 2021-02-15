using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchesPopupAnimator : MonoBehaviour
{
    public GameTypeController gameType;

    public void RightButton()
    {
        /*foreach(var aux in this.GetComponentsInChildren<Animator>())
        {
            aux.Play("Right", -1, 0f);
        }*/
        if (gameType.gameSelected + 1 < gameType.transform.childCount)
        {
            gameType.gameSelected++;
        }
        else
        {
            gameType.gameSelected = 0;
        }
        EnableChild();
    }

    public void LeftButton()
    {
        /*foreach (var aux in this.GetComponentsInChildren<Animator>())
        {
            aux.Play("Left", -1, 0f);
        }*/
        if (gameType.gameSelected - 1 > 0)
        {
            gameType.gameSelected--;
        }
        else
        {
            gameType.gameSelected = gameType.transform.childCount-1;
        }
        EnableChild();
    }

    public void EnableChild()
    {
        foreach (Transform aux in this.transform)
        {
            aux.gameObject.SetActive(false);
        }
        this.transform.GetChild(gameType.gameSelected).gameObject.SetActive(true);
    }
}
