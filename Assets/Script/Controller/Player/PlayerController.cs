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

    public BotController botController;

    private Animator animator;

    public bool freeBoat = false;

    public bool fakeTravel = false;

    public bool stayAway = false;

    public PlayerDg playerDg;

    private void Update()
    {
        canvasController.freeBoatIcon.SetActive(freeBoat);
    }

    public void SetupStart(Player player, bool isBot = false, int botNumber = 0, string botName = "")
    {
        this.GetComponent<PhotonView>().RPC("SetupStart_CMD", RpcTarget.All, player, isBot, botNumber, botName);

        int index = Random.Range(0, manager.availablePlayers.Count);
        photonView.RPC("SetupPlayerDg_CMD", RpcTarget.All, index);
    }
    
    [PunRPC]
    public void SetupStart_CMD(Player player, bool isBot, int botNumber, string botName)
    {
        photonView = this.GetComponent<PhotonView>();

        if(isBot)
        {
            this.gameObject.AddComponent<BotController>();
        }

        this.player = player;
        string nickname = string.IsNullOrEmpty(player?.NickName) ? botName : player.NickName;

        playerNumber = isBot ? botNumber : photonView.ControllerActorNr;
        this.transform.name = nickname;

        walletController = this.GetComponent<PlayerWalletController>();
        walletController.controller = this;

        moveController = this.GetComponent<PlayerMoveController>();
        moveController.playerController = this;

        manager = FindObjectOfType<GameManager>();
        boardController = FindObjectOfType<BoardController>();

        GameObject objCanvas = manager.canvasManager.playerCanvas[playerNumber-1];
        objCanvas.SetActive(true);
        canvasController = objCanvas.GetComponent<PlayerControllerCanvas>();
        canvasController.ConfigureUI(nickname, walletController.currentMoney);
        canvasController.player = this;

        btnThrowDice = manager.canvasManager.btnThrow;

        Color color = manager.playerColors[playerNumber - 1];

        photonView.RPC("SetupMaterial", RpcTarget.All, new Vector3(color.r, color.g, color.b));

        if (isBot && !PhotonNetwork.IsMasterClient) Manager.instance.bots++;
        manager.NewPlayer(this);
    }

    [PunRPC]
    public void SetupPlayerDg_CMD(int index)
    {
        this.playerDg = manager.availablePlayers[index];

        Vector3 modelPos = this.transform.Find("Model").position;
        Vector3 modelScale = this.transform.Find("Model").localScale;
        Quaternion modelRot = this.transform.Find("Model").rotation;

        Destroy(this.transform.Find("Model").gameObject);
        var model = Instantiate(playerDg.model, modelPos, modelRot, this.transform);
        model.transform.localScale = modelScale;
        model.name = "Model";
        model.transform.SetSiblingIndex(0);

        animator = model.GetComponent<Animator>();

        Transform icon = this.transform.Find("Icon");
        icon.GetComponent<SpriteRenderer>().sprite = playerDg.icon;
        icon.GetChild(0).GetComponent<SpriteRenderer>().color = mainColor;

        canvasController.ConfigureSprite(playerDg.icon);
    }

    [PunRPC]
    public void SetupMaterial(Vector3 color)
    {
        this.mainColor = new Color(color.x, color.y, color.z);

        var newMat = new Material(this.GetComponent<MeshRenderer>().sharedMaterials[0]);
        newMat.color = this.mainColor;
        List<Material> mats = new List<Material>() { newMat };
        this.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();

        //btnThrowDice.image.color = this.mainColor;
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1, 7);
    }

    public IEnumerator ConfigDice()
    {
        if (MessageManager.Instance.TextShowing())
        {
            yield return new WaitForSeconds(3.0f);
            MessageManager.Instance.HiddenText();
        }
        yield return new WaitForSeconds(0.2f);

        if (stayAway)
        {
            stayAway = false;
            this.photonView.RPC("NextPlayer_CMD", RpcTarget.All);
            yield break;
        }

        if (!botController)
        {
            this.btnThrowDice.interactable = true;
            this.btnThrowDice.gameObject.SetActive(true);

            this.btnThrowDice.onClick.RemoveAllListeners();
            this.btnThrowDice.onClick.AddListener(() =>
            {
                int dice1 = ThrowDice();//dice1special;
                int dice2 = ThrowDice();//dice2special;

                photonView.RPC("RollDice_CMD", RpcTarget.All, dice1, dice2);
                this.moveController.StartMovePlayer(dice1, dice2);
                this.btnThrowDice.interactable = false;
                this.btnThrowDice.gameObject.SetActive(false);
            });
            //this.btnThrowDice.image.color = mainColor;
        }
        else
        {
            //BOT
            StartCoroutine(botController.ExecuteAction(() =>
            {
                int dice1 = 7;//ThrowDice();//ThrowDice();
                int dice2 = 9;//ThrowDice();//ThrowDice();

                photonView.RPC("RollDice_CMD", RpcTarget.All, dice1, dice2);
                this.moveController.StartMovePlayer(dice1, dice2);
            }));
        }
    }

    public int dice1special, dice2special;

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
        var desiredAngle = Quaternion.Euler(newRot.x, newRot.y + 90, newRot.z);

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

        photonView.RPC("TravelPlayer_CMD", RpcTarget.All, tile.index);
    }

    [PunRPC]
    public void TravelPlayer_CMD(int index)
    {
        canTravel = false;
        int value = 0;

        moveController.position = currentTile.index;

        if (moveController.position > index)
        {
            value = (boardController.tileControllers.Count - moveController.position) + (index - 1);
        }
        else
        {
            value = index - moveController.position;
        }
        boardController.ResetBoard();

        StartCoroutine(manager.OnMovePlayer(moveController, moveController.MovePlayer(value), false));
    }

    public void GotoJail()
    {
        this.jailInTotal++;
        if (this.jailInTotal > 4)
        {
            manager.playerDefetead = true;
            DeclareBankruptcy();
        }
        this.inJail = true;
        this.jailRow = 0;

        photonView.RPC("SetCurrentTile_CMD", RpcTarget.All, boardController.jail.index);
        photonView.RPC("TeleportToJail_CMD", RpcTarget.All);
        moveController.position = boardController.jail.index;

        if(freeBoat)
        {
            this.inJail = false;
        }
    }

    [PunRPC]
    public void TeleportToJail_CMD()
    {
        Vector3 newPos = boardController.jail.transform.position;
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;

        MessageManager.Instance.ShowMessage("<u>"+this.transform.name + "</u> se perdeu no triangulo das bermudas");

        StartCoroutine(manager.TestPlayerOnSameHouse(this.moveController));
    }

    public void GotoTile(TileController tile, string tileName, bool vacation = false, bool jail = false)
    {
        photonView.RPC("TeleportToTile_CMD", RpcTarget.All, tile.index, tileName, vacation, jail);
    }

    [PunRPC]
    public void TeleportToTile_CMD(int tileIndex, string tileName, bool vacation = false, bool jail = false)
    {
        TileController tile = manager.board.tileControllers.Find(n => n.index == tileIndex);
        currentTile = tile;
        Vector3 newPos = tile.transform.position;
        newPos.y = this.transform.position.y;
        this.transform.position = newPos;

        moveController.position = tile.index;
        canTravel = vacation;
        inJail = jail;

        var rot = this.transform.rotation.eulerAngles;
        rot.y = tile.cornerRotation!=5?tile.cornerRotation:rot.y;
        this.transform.rotation = Quaternion.Euler(rot);

        if (!inJail)
        {
            MessageManager.Instance.ShowMessage("<u>" + this.transform.name + "</u> foi para " + tileName);
        }
        else
        {
            MessageManager.Instance.ShowMessage("<u>" + this.transform.name + "</u> se perdeu no triangulo das bermudas");
        }

        StartCoroutine(manager.TestPlayerOnSameHouse(this.moveController));
    }

    public void DeclareBankruptcy()
    {
        manager.players.Remove(this);
        canvasController.DeclareBankruptcy();

        MessageManager.Instance.ShowMessage("<u>" + transform.name + "</u> declarou falencia");

        foreach (var aux in properties)
        {
            aux.Owner = null;
        }
    }

    public void Animate_Walk()
    {
        animator.SetTrigger("_move");
    }

    public void Animate_Bad()
    {
        animator.SetTrigger("_bad");
    }

    public void Animate_Cheer()
    {
        animator.SetTrigger("_cheer");
    }

    [PunRPC]
    public void SetCurrentTile_CMD(int index)
    {
        currentTile = manager.board.tileControllers.Find(n => n.index == index);
    }

    public void WinGame()
    {
        //Debug.Log((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);

        MessageManager.Instance.ShowMessage("<u>" + this.transform.name + "</u> ganhou o jogo");
        Debug.Log("Player win game " + this.transform.name);
    }


    [PunRPC]
    public void EnableModel_CMD()
    {
        this.transform.Find("Model").gameObject.SetActive(true);
        this.transform.Find("Icon").gameObject.SetActive(false);
    }

    [PunRPC]
    public void SetupFreeBoat_CMD(bool state)
    {
        this.freeBoat = state;
    }

    [PunRPC]
    public void RollDice_CMD(int dice1, int dice2)
    {
        StartCoroutine(manager.RollDice(dice1, dice2, playerNumber));
    }
}