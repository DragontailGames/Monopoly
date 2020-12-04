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

    public List<TileController_Buyable> properties = new List<TileController_Buyable>();

    public int currentMoney = 3000000;

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

    public bool testedBankruptcy = false;

    public List<Doubt> doubts = new List<Doubt>();

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

    public void MortgagePropertie(TileController tileController)
    {
        TileController_Buyable btile = tileController as TileController_Buyable;
        properties.Remove(btile);
        CreditValue(Math.GetMortgagePrice(btile));
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

    public void DebitValue(int value)
    {
        StartCoroutine(canvasController.DebitAnimation(value, currentMoney));
        currentMoney -= value;
    }

    public void CreditValue(int value)
    {
        StartCoroutine(canvasController.CreditAnimation(value, currentMoney));
        currentMoney += value;
    }

    public void TransferMoney(int debitMoney, int creditMoney, PlayerController otherPlayer)
    {
        if(currentMoney-debitMoney<0)
        {
            doubts.Add(new Doubt()
            {
                value = debitMoney - currentMoney,
                target = otherPlayer
            });

            creditMoney -= currentMoney;
            otherPlayer.CreditValue(creditMoney);

            this.DebitValue(currentMoney);
        }
        else
        {
            this.DebitValue(debitMoney);
            otherPlayer.CreditValue(creditMoney);
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
        foreach(var aux in properties)
        {
            aux.owner = null;
        }
    }

    public bool ExitingDoubts()
    {
        var existingDoubles = doubts.FindAll(n => n.value > 0);
        return existingDoubles.Count>0;
    }

    public IEnumerator CheckBankruptcy()
    {
        testedBankruptcy = false;
        if(ExitingDoubts())
        {
            if (properties.Count > 0)
                boardController.SetupMortgageBoard(this);
            else
            {
                manager.PlayerDefeated();
                boardController.ResetBoard();
                DeclareBankruptcy();
                testedBankruptcy = true;
            }
        }
        else
        {
            boardController.ResetBoard();
            testedBankruptcy = true;
        }
        yield return new WaitUntil (() => testedBankruptcy == true);
    }

    public void PayDoubts()
    {
        foreach(var aux in doubts)
        {
            TransferMoney(aux.value, aux.value, aux.target);
            aux.value = 0;
        }
        for(int i = doubts.Count-1;i>=0;i--)
        {
            if (doubts[i].value<=0)
            {
                doubts.RemoveAt(i);
            }
        }
        StartCoroutine(CheckBankruptcy());
    }

    public void WinGame()
    {
        Debug.Log("Player win game " + this.transform.name);
    }
}