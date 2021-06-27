using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableMenuController : MonoBehaviour
{
    public GameObject focusPanel;

    public BuyableHouseMenuController buyableHouseMenuController;

    public BuyableRentMenuController buyableRentMenuController;

    public BuyableWonderMenuController buyableWonderMenuController;

    public BuyableRentWonderMenuController buyableRentWonderMenuController;

    public IEnumerator SetupMenu(TileController_Buyable tile, PlayerController player)
    {
        if (!player.botController)
        {
            focusPanel.SetActive(true);
        }
        if (tile.GetType() == typeof(TileController_Country))
        {
            if (tile.Owner == player || tile.Owner == null)
            {
                yield return buyableHouseMenuController.SetupUpgradeTile(tile as TileController_Country, player);
            }
            else
            {
                yield return buyableRentMenuController.SetupRentTile(tile as TileController_Country, player);
            }
        }
        else
        {

            if (tile.Owner == null)
            {
                yield return buyableWonderMenuController.SetupWonderTile(tile as TileController_Wonders, player);
            }
            else if (tile.Owner != player)
            {
                yield return buyableRentWonderMenuController.SetupRentWonderTile(tile as TileController_Wonders, player);
            }
        }

        focusPanel.SetActive(false);
    }
}
