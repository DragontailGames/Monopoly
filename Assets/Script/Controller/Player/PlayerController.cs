using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerController : MonoBehaviour
{
    public GameObject controllerCanvasPrefab;

    [HideInInspector]
    public PlayerControllerCanvas canvasController;

    [HideInInspector]
    public PlayerMoveController moveController;

    [HideInInspector]
    public PlayerWalletController walletController;

    public PhotonView photonView;

    public Button btnThrowDice;

    public int playerNumber;

    [HideInInspector]
    public BoardController boardController;

    [HideInInspector]
    public GameManager manager;

    public TileController currentTile;

    [HideInInspector]
    public List<TileController_Buyable> properties = new List<TileController_Buyable>();

    [HideInInspector]
    public int wondersInControl;

    [HideInInspector]
    public bool canTravel = false;

    [HideInInspector]
    public bool inJail = false;

    [HideInInspector]
    public int doubleRow = 0;

    [HideInInspector]
    public int jailRow = 0;

    [HideInInspector]
    public int jailInTotal = 0;

    [HideInInspector]
    public bool firstBuy = false;

    public Color mainColor;

    public Player player;

    public void SetupStart(Player player)
    {
        this.GetComponent<PhotonView>().RPC("SetupStart_CMD", RpcTarget.All, player);
    }
    
    [PunRPC]
    public void SetupStart_CMD(Player player)
    {
        photonView = this.GetComponent<PhotonView>();

        this.player = player;

        walletController = this.GetComponent<PlayerWalletController>();
        walletController.controller = this;

        moveController = this.GetComponent<PlayerMoveController>();
        moveController.playerController = this;

        manager = FindObjectOfType<GameManager>();
        boardController = FindObjectOfType<BoardController>();

        manager.NewPlayer(this);
    }

    public void ConfigUI()
    {
        photonView.RPC("ConfigUI_CMD", RpcTarget.All);
    }

    [PunRPC]
    public void ConfigUI_CMD()
    {
        this.transform.name = "Player_" + playerNumber;

        GameObject objCanvas = manager.canvasManager.playerCanvas[playerNumber];
        objCanvas.SetActive(true);
        canvasController = objCanvas.GetComponent<PlayerControllerCanvas>();
        canvasController.ConfigureUI(null, "Player_" + playerNumber, walletController.currentMoney);
        canvasController.player = this;

        btnThrowDice = manager.canvasManager.btnThrow;

        if (player.IsLocal)
        {
            Color color = Random.ColorHSV();
            photonView.RPC("SetupMaterial", RpcTarget.All, new Vector3(color.r, color.g, color.b));
        }
    }

    [PunRPC]
    public void SetupMaterial(Vector3 color)
    {
        this.mainColor = new Color(color.x, color.y, color.z);

        var newMat = new Material(this.GetComponent<MeshRenderer>().sharedMaterials[0]);
        newMat.color = this.mainColor;
        List<Material> mats = new List<Material>() { newMat };
        this.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();

        btnThrowDice.image.color = this.mainColor;

        canvasController.icon.color = this.mainColor;
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1, 7);
    }

    public IEnumerator ConfigDice()
    {
        yield return new WaitForSeconds(0.2f);
        photonView.RPC("ConfigDice_CMD", RpcTarget.All);
    }

    [PunRPC]
    public void ConfigDice_CMD()
    {
        if (player.IsLocal)
        {
            this.btnThrowDice.interactable = true;
            this.btnThrowDice.gameObject.SetActive(true);

            this.btnThrowDice.onClick.RemoveAllListeners();
            this.btnThrowDice.onClick.AddListener(() => {
                this.moveController.StartMovePlayer(ThrowDice(), ThrowDice());
                this.btnThrowDice.interactable = false;
                this.btnThrowDice.gameObject.SetActive(false);
            });
            this.btnThrowDice.image.color = mainColor;
        }
    }

    public void MortgagePropertie(TileController tileController)
    {
        TileController_Buyable btile = tileController as TileController_Buyable;
        properties.Remove(btile);
        walletController.CreditValue(MathDt.GetMortgagePrice(btile));
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
        var newRot = this.transform.localEulerAngles;
        var startY = newRot.y;
        var desiredAngle = Quaternion.Euler(newRot.x, newRot.y - 90, newRot.z);

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
            var transparentMainColor = mainColor;
            transparentMainColor.a = 0.6f;
            this.GetComponent<MeshRenderer>().sharedMaterials[0].color = transparentMainColor;
        }
        else
        {
            this.GetComponent<MeshRenderer>().sharedMaterials[0].color = mainColor;
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