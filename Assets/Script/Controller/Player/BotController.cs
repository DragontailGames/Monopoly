using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotController : MonoBehaviour
{

    public EnumDt.botBehavior botBehavior;

    public PlayerController player;

    public void Start()
    {
        player = this.GetComponent<PlayerController>();
        player.walletController.whenUpdateMoney = () => WhenUpdateMoney();
    }

    public void ExecuteAction(UnityAction eventNormal, UnityAction eventEconomic = null, UnityAction eventRich = null)
    {
        if(botBehavior == EnumDt.botBehavior.economic && eventEconomic != null)
        {
            eventEconomic?.Invoke();
        }
        else if (botBehavior == EnumDt.botBehavior.rich && eventRich != null)
        {
            eventEconomic?.Invoke();
        }
        else
        {
            eventNormal?.Invoke();
        }
    }

    public void WhenUpdateMoney()
    {
        if(player.walletController.currentMoney < MathDt.startMoney * 0.3f)
        {
            botBehavior = EnumDt.botBehavior.economic;
        }
        else if (player.walletController.currentMoney < MathDt.startMoney * 0.7f)
        {
            botBehavior = EnumDt.botBehavior.normal;
        }
        else
        {
            botBehavior = EnumDt.botBehavior.rich;
        }
    }
}
