using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : MonoBehaviour
{
    public GameObject controllerCanvasPrefab;

    [HideInInspector]
    public PlayerControllerCanvas canvasController;

    [HideInInspector]
    public PlayerMoveController moveController;

    [HideInInspector]
    public PlayerWalletController walletController;

    [HideInInspector]
    public NetworkIdentity networkIdentity;

    public int playerNumber;

    public BoardController boardController;

    public GameManager manager;

    public Material normal, transparent;

    public TileController currentTile;

    public List<TileController_Buyable> properties = new List<TileController_Buyable>();

    [HideInInspector]
    public int wondersInControl;

    [HideInInspector]
    public bool canTravel = false;

    [HideInInspector]
    public bool inJail = false;

    public int doubleRow = 0;

    public int jailRow = 0;

    public int jailInTotal = 0;

    public bool firstBuy = false;

    public Color mainColor;

    public void Awake()
    {
        walletController = this.GetComponent<PlayerWalletController>();
        walletController.controller = this;
        walletController.canvasController = canvasController;

        moveController = this.GetComponent<PlayerMoveController>();
        moveController.playerController = this;

        networkIdentity = this.GetComponent<NetworkIdentity>();

        manager = GameObject.FindObjectOfType<GameManager>();
        manager.players.Add(this);
        playerNumber = manager.players.FindIndex(n => n == this);
    }

    public void Start()
    {
        GameObject objCanvas = Instantiate(controllerCanvasPrefab, manager.canvasManager.transform);
        canvasController = objCanvas.GetComponent<PlayerControllerCanvas>();
        canvasController.player = this;
        canvasController.ConfigurePosition();
        canvasController.ConfigureUI(null, "Player_" + networkIdentity.netId, walletController.currentMoney);
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1, 7);
    }

    public void MortgagePropertie(TileController tileController)
    {
        TileController_Buyable btile = tileController as TileController_Buyable;
        properties.Remove(btile);
        walletController.CreditValue(Math.GetMortgagePrice(btile));
    }

    public void WonderWin()
    {
        if(wondersInControl>=5)
        {
            WinGame();
        }
    }

    public void CheckWin()
    {
        manager.CheckWinSide(this);
        manager.CheckWinBlocks(this);
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
        boardController.ResetBoard();
        StartCoroutine(manager.OnMovePlayer(moveController, moveController.MovePlayer(value), false));
    }

    public void TeleportPlayer(TileController tile)
    {
        Vector3 newPos = tile.transform.position;
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;
        currentTile = tile;
        moveController.position = tile.index;
        canTravel = false;
        //StartCoroutine(manager.OnMovePlayer(this, StartCoroutine(MovePlayer(0)), false));
    }

    public void GotoJail()
    {
        this.jailInTotal++;
        if(this.jailInTotal>4)
        {
            manager.PlayerDefeated();
            DeclareBankruptcy();
        }
        this.inJail = true;
        this.jailRow = 0;
        this.TeleportPlayer(boardController.jail);
    }

    public void DeclareBankruptcy()
    {
        manager.players.Remove(this);
        canvasController.DeclareBankruptcy();
        foreach (var aux in properties)
        {
            aux.Owner = null;
        }
    }

    public void WinGame()
    {
        //Debug.Log((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        Debug.Log("Player win game " + this.transform.name);
    }
}