using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class PlayerWalletController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController controller;

    public int currentMoney;

    public bool testedBankruptcy = false;

    public List<Doubt> doubts = new List<Doubt>();

    public UnityAction whenUpdateMoney;

    public void Start()
    {
        currentMoney = MathDt.startMoney;
    }

    public void DebitValue(int value)
    {
        controller.photonView.RPC("DebitValue_CMD", RpcTarget.All, value);
    }

    [PunRPC]
    private void DebitValue_CMD(int value)
    {
        StartCoroutine(controller.canvasController.DebitAnimation(value, currentMoney));
        currentMoney -= value;
        whenUpdateMoney?.Invoke();
    }

    public void CreditValue(int value)
    {
        controller.photonView.RPC("CreditValue_CMD", RpcTarget.All, value);
    }

    [PunRPC]
    private void CreditValue_CMD(int value)
    {
        StartCoroutine(controller.canvasController.CreditAnimation(value, currentMoney));
        currentMoney += value;
        whenUpdateMoney?.Invoke();
    }

    public void TransferMoney(int debitMoney, int creditMoney, PlayerController otherPlayer)
    {
        if (currentMoney - debitMoney < 0)
        {
            doubts.Add(new Doubt()
            {
                value = debitMoney - currentMoney,
                target = otherPlayer
            });

            creditMoney -= currentMoney;
            otherPlayer.walletController.CreditValue(creditMoney);

            this.DebitValue(currentMoney);
        }
        else
        {
            this.DebitValue(debitMoney);
            otherPlayer.walletController.CreditValue(creditMoney);
        }

        whenUpdateMoney?.Invoke();
    }

    public bool ExitingDoubts()
    {
        var existingDoubles = doubts.FindAll(n => n.value > 0);
        return existingDoubles.Count > 0;
    }

    public IEnumerator CheckBankruptcy()
    {
        testedBankruptcy = false;
        if (ExitingDoubts())
        {
            if (controller.properties.Count > 0)
            {
                controller.boardController.SetupMortgageBoard(controller);
                controller.LogMessagePlayer($"{controller.name} não possui dinheiro e deve hipotecar uma propriedade!",true);
            }
            else
            {
                controller.manager.playerDefetead = true;
                controller.boardController.ResetBoard();
                controller.DeclareBankruptcy();
                testedBankruptcy = true;
            }
        }
        else
        {
            testedBankruptcy = true;
        }
        yield return new WaitUntil(() => testedBankruptcy == true);
    }

    public void PayDoubts()
    {
        foreach (var aux in doubts)
        {
            TransferMoney(aux.value, aux.value, aux.target);
            aux.value = 0;
        }
        for (int i = doubts.Count - 1; i >= 0; i--)
        {
            if (doubts[i].value <= 0)
            {
                doubts.RemoveAt(i);
            }
        }
        StartCoroutine(CheckBankruptcy());
    }

    [PunRPC]
    public void BuyTile_CMD(int index, string txt, bool hostile)
    {
        TileController_Buyable tile = controller.manager.board.tileControllers.Find(n => n.index == index) as TileController_Buyable;

        if (((controller.player != null && !controller.player.IsLocal) || controller.botController) && txt != "")
        {
            if (hostile)
            {
                MessageManager.Instance.ShowMessage($"{txt}");
            }
            else
            {
                MessageManager.Instance.ShowMessage($"{controller.name} comprou {txt}");
            }
        }

        tile.Owner = controller;
        controller.properties.Add(tile);

        controller.CheckWin();
        tile.OnBuy(controller);
    }

    [PunRPC]
    public void UpgradeLevel_CMD(int level, int index, bool hasOwner)
    {
        TileController_Country tile = controller.manager.board.tileControllers.Find(n => n.index == index) as TileController_Country;

        tile.level = level;

        tile.SetupBuilding(hasOwner);
    }
}
