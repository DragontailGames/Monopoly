using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerControllerCanvas canvasController;

    public PlayerMoveController moveController;

    public BoardController boardController;

    public GameManager manager;

    public Material normal, transparent;

    public TileController currentTile;

    public int currentMoney = 3000000;

    [HideInInspector]
    public int wondersInControl;

    [HideInInspector]
    public bool canTeleport = false;

    [HideInInspector]
    public bool inJail = false;

    public int doubleRow = 0;

    public int jailRow = 0;


    public void Awake()
    {
        manager.players.Add(this);
    }

    public void Start()
    {
        canvasController.ConfigureUI(null, "Player_" + Random.Range(1000, 9999), currentMoney);
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1, 7);
    }

    public IEnumerator TurnCorner()
    {
        var newRot = this.transform.rotation;
        var startY = newRot.y;
        var desiredAngle = Quaternion.Euler(newRot.x, newRot.y - 90, newRot.y);

        yield return new WaitUntil(() =>
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, desiredAngle, Time.deltaTime * 20.0f);
            if (Vector3.Distance(this.transform.rotation.eulerAngles, desiredAngle.eulerAngles) < 1f)
            {
                return true;
            }
            return false;
        });
    }

    public void ChangeMaterial(bool transparent)
    {
        if (transparent)
        {
            this.transform.GetComponent<MeshRenderer>().material = this.transparent;
        }
        else
        {
            this.transform.GetComponent<MeshRenderer>().material = normal;
        }
    }

    public void DebitValue(int value)
    {
        currentMoney -= value;
        canvasController.UpdateMoney(currentMoney);
    }

    public void CreditValue(int value)
    {
        currentMoney += value;
        canvasController.UpdateMoney(currentMoney);
    }

    public void TravelPlayer(TileController tile)
    {
        int value = 0;

        if (moveController.position>tile.index)
        {
            value = (boardController.tileControllers.Count - moveController.position) + (tile.index -1) ;
        }
        else
        {
            value = tile.index - moveController.position;
        }
        StartCoroutine(manager.OnMovePlayer(moveController, moveController.MovePlayer(value), false));
    }

    public void TeleportPlayer(TileController tile)
    {
        Vector3 newPos = tile.transform.position;
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;
        currentTile = tile;
        moveController.position = tile.index;
        canTeleport = false;
        //StartCoroutine(manager.OnMovePlayer(this, StartCoroutine(MovePlayer(0)), false));
    }

    public void GotoJail()
    {
        this.inJail = true;
        this.jailRow = 0;
        this.TeleportPlayer(boardController.jail);
    }
}