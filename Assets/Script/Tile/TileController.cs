using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int index;

    public Tile tile;

    public bool tileIsTeleported = true;

    public PlayerController playerToTeleport;

    public BoardController boardController;

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
        if (playerToTeleport && tileIsTeleported)
        {
            playerToTeleport.TravelPlayer(this);
            boardController.ResetPlayerToTeleport();
        }
    }

    public void SetupTeleport(PlayerController player)
    {
        playerToTeleport = player;
        if (!tileIsTeleported)
        {
            var baseColor = this.GetComponent<SpriteRenderer>().color;
            baseColor.a = 0.4f;
            this.GetComponent<SpriteRenderer>().color = baseColor;
        }
    }
}