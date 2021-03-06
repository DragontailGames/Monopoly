﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class TileController : MonoBehaviour
{
    public int index;

    public Tile tile;

    private bool colorChanged = false;

    public bool tileIsTeleported = true;

    public BoardController boardController;

    public UnityAction onClickAction;

    public float offsetZ = -0.17f;

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
                if (player.walletController.ExitingDoubts())
                {
                    player.walletController.PayDoubts();
                }
                else
                {
                    player.walletController.testedBankruptcy = true;
                }
                onClickAction = null;
            };
        }
        else
        {
            SetupOffTiles();
        }
    }

    public TileController SetupPropertieLucky(PlayerController player, UnityAction<TileController> action, bool ownerTile, bool toBot, bool onlyCountry)
    {
        if (player.properties.Contains(this as TileController_Buyable) == ownerTile  &&
            (this.GetType() == typeof(TileController_Country) ||
            (this.GetType() == typeof(TileController_Wonders) && !onlyCountry)))

        {
            if (!ownerTile && (this as TileController_Buyable).Owner == null)
            {
                SetupOffTiles();
                return null;
            }
            onClickAction = () => action.Invoke(this);
            return this;
        }
        else
        {
            if(!toBot)
                SetupOffTiles();
            return null;
        }
    }

    public void SetupOffTiles()
    {
        Material[] mtList = this.transform.Find("Base").GetComponent<MeshRenderer>().sharedMaterials;

        List <Material> newList = new List<Material>();

        foreach (var auxMaterial in mtList)
        {
            var mat = new Material(auxMaterial);
            var color = mat.color;

            color = color / 3;
            color.a = 0.1f;

            mat.color = color;
            newList.Add(mat);
        }

        colorChanged = true;

        this.transform.Find("Base").GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
    }

    public void ResetTile()
    {
        if (!colorChanged)
            return;

        Material[] mtList = this.transform.Find("Base").GetComponent<MeshRenderer>().sharedMaterials;
        List<Material> newList = new List<Material>();

        foreach (var auxMaterial in mtList)
        {
            var mat = new Material(auxMaterial);
            var color = mat.color;
            color = color * 3;
            color.a = 1.0f;
            mat.color = color;
            newList.Add(mat);
        }

        this.transform.Find("Base").GetComponent<MeshRenderer>().sharedMaterials = newList.ToArray();
    }

    public Vector3 GetPosition()
    {
        Vector3 newPos = this.transform.position;
        newPos.y = 0.35f;

        return newPos;
    }

    public virtual void OnTurnStart()
    {

    }

    public virtual void OnTurnEnd()
    {

    }
}