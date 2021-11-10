using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckyMenuController : MonoBehaviour
{
    public TextMeshProUGUI luckyDescription;

    bool clicked = false;

    public Sprite iconLuck, iconReverse;

    public Image icon;

    public AudioClip luckyAudio;

    public AudioClip reverseAudio;

    public AudioSource audioSource;

    public IEnumerator LuckyStart(TileLucky lucky, PlayerController player)
    {
        if(!player.botController)
            this.gameObject.SetActive(true);

        clicked = false;

        bool isLucky = lucky.luckType == EnumDt.luckType.luck;

        icon.sprite = isLucky ? iconLuck : iconReverse;

        if(isLucky)
        {
            audioSource.PlayOneShot(luckyAudio);
        }
        else
        {
            audioSource.PlayOneShot(reverseAudio);
        }

        luckyDescription.text = lucky.text;

        if (player.botController)
        {
            yield return player.botController.ExecuteAction(() => { clicked = true; });
        }

        yield return new WaitUntil(() => clicked == true);
    }

    public void ClickBackButton()
    {
        clicked = true;
        this.gameObject.SetActive(false);
    }
}
