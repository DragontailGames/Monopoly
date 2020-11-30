using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class TileController : MonoBehaviour
{
    public int index;

    public Tile tile;

    public bool tileIsTeleported = true;

    public BoardController boardController;

    public UnityAction onClickAction;

    public virtual IEnumerator OnPlayerPass(PlayerController player)
    {
        yield return new WaitForSeconds(0.05f);
    }

    public virtual IEnumerator OnPlayerStop(PlayerController player)
    {
        yield return null;
    }

    public void OnMouseDown()
    {
        onClickAction?.Invoke();
    }

    public void SetupTravel(PlayerController player)
    {
        if (tileIsTeleported)
        {
            onClickAction = () => player.TravelPlayer(this);
        }
        else
        {
            SetupOffTiles();
        }
    }

    public void SetupMortgage(PlayerController player)
    {
        if (player.properties.Contains(this as TileController_Buyable))
        {
            onClickAction = () =>
            {
                player.MortgagePropertie(this);
                if (player.ExitingDoubts())
                {
                    player.PayDoubts();
                }
                else
                {
                    player.testedBankruptcy = true;
                }
                onClickAction = null;
            };
        }
        else
        {
            SetupOffTiles();
        }
    }

    public void SetupOffTiles()
    {
        var baseColor = this.GetComponent<SpriteRenderer>().color;
        baseColor.a = 0.4f;
        this.GetComponent<SpriteRenderer>().color = baseColor;
    }

    public void ResetTile()
    {
        var baseColor = this.GetComponent<SpriteRenderer>().color;
        baseColor.a = 1f;
        this.GetComponent<SpriteRenderer>().color = baseColor;
    }
}