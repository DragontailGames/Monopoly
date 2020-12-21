using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalletController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController controller;

    [HideInInspector]
    public PlayerControllerCanvas canvasController;

    public int currentMoney = 3000000;

    public bool testedBankruptcy = false;

    public List<Doubt> doubts = new List<Doubt>();

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
                controller.boardController.SetupMortgageBoard(controller);
            else
            {
                controller.manager.PlayerDefeated();
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
}
