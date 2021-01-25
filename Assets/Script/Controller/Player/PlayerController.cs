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

    public bool canTravel = false;

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

        playerNumber = photonView.ControllerActorNr;
        this.transform.name = "Player_" + playerNumber;

        walletController = this.GetComponent<PlayerWalletController>();
        walletController.controller = this;

        moveController = this.GetComponent<PlayerMoveController>();
        moveController.playerController = this;

        manager = FindObjectOfType<GameManager>();
        boardController = FindObjectOfType<BoardController>();

        GameObject objCanvas = manager.canvasManager.playerCanvas[playerNumber-1];
        objCanvas.SetActive(true);
        canvasController = objCanvas.GetComponent<PlayerControllerCanvas>();
        canvasController.ConfigureUI(null, "Player_" + playerNumber, walletController.currentMoney);
        canvasController.player = this;

        btnThrowDice = manager.canvasManager.btnThrow;

        if (player.IsLocal)
        {
            Color color = new Color(
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f));
            photonView.RPC("SetupMaterial", RpcTarget.All, new Vector3(color.r, color.g, color.b));
        }
        manager.NewPlayer(this);
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
        Debug.Log("Chamou os dados", this);
        if (player.IsLocal)
        {
            this.btnThrowDice.interactable = true;
            this.btnThrowDice.gameObject.SetActive(true);

            this.btnThrowDice.onClick.RemoveAllListeners();
            this.btnThrowDice.onClick.AddListener(() => {

                int dice1 = int.Parse(GameObject.Find("Dice 1").GetComponent<TMPro.TMP_InputField>().text);
                int dice2 = int.Parse(GameObject.Find("Dice 2").GetComponent<TMPro.TMP_InputField>().text);

                this.moveController.StartMovePlayer(dice1, dice2);
                //PEDROthis.moveController.StartMovePlayer(ThrowDice(), ThrowDice());
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
        if (canTravel == false)
            return;

        int value = 0;
        canTravel = false;

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

    public void GotoJail()
    {
        this.jailInTotal++;
        if (this.jailInTotal > 4)
        {
            manager.PlayerDefeated();
            DeclareBankruptcy();
        }
        this.inJail = true;
        this.jailRow = 0;

        photonView.RPC("SetCurrentTile_CMD", RpcTarget.All, boardController.jail.index);
        photonView.RPC("TeleportToJail_CMD", RpcTarget.All);
        moveController.position = boardController.jail.index;
    }

    [PunRPC]
    public void TeleportToJail_CMD()
    {
        Vector3 newPos = boardController.jail.transform.position;
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;

        StartCoroutine(manager.TestPlayerOnSameHouse(this.moveController));
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

    [PunRPC]
    public void SetCurrentTile_CMD(int index)
    {
        currentTile = manager.board.tileControllers.Find(n => n.index == index);
    }

    public void WinGame()
    {
        //Debug.Log((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        Debug.Log("Player win game " + this.transform.name);
    }
}