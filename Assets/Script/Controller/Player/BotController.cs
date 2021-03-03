using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotController : MonoBehaviour
{
    public EnumDt.botBehavior botBehavior = EnumDt.botBehavior.rich;

    public PlayerController player;

    public void Start()
    {
        player = this.GetComponent<PlayerController>();
        player.walletController.whenUpdateMoney = () => WhenUpdateMoney();
        player.botController = this;
    }

    public IEnumerator ExecuteAction(UnityAction eventNormal, UnityAction eventEconomic = null, UnityAction eventRich = null)
    {
        if(botBehavior == EnumDt.botBehavior.economic && eventEconomic != null)
        {
            yield return StartCoroutine(Action(eventEconomic));
        }
        else if (botBehavior == EnumDt.botBehavior.rich && eventRich != null)
        {
            yield return StartCoroutine(Action(eventRich));
        }
        else
        {
            yield return StartCoroutine(Action(eventNormal));
        }
    }

    public IEnumerator Action(UnityAction action)
    {
        yield return new WaitForSeconds(2.0f);
        action?.Invoke();
    }

    public void WhenUpdateMoney()
    {
        if(player.walletController.currentMoney < MathDt.startMoney * 0.4f)
        {
            botBehavior = EnumDt.botBehavior.economic;
        }
        else if (player.walletController.currentMoney < MathDt.startMoney * 0.9f)
        {
            botBehavior = EnumDt.botBehavior.normal;
        }
        else
        {
            botBehavior = EnumDt.botBehavior.rich;
        }
    }
}
