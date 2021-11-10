using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    close,
    open,
    buy,
    diceRoll,
    fail,
    lose,
    move,
    pay,
    success,
}

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip close_btn;

    public AudioClip open_btn;

    public AudioClip buy;

    public AudioClip diceroll;

    public AudioClip fail;

    public AudioClip lose;

    public AudioClip move;

    public AudioClip pay;

    public AudioClip success;

    private void Start()
    {
        Manager.instance.audioManager = this;
    }

    public void PlayAudio(AudioType audioType)
    {
        switch(audioType)
        {
            case AudioType.close:
                audioSource.PlayOneShot(close_btn);
                break;
            case AudioType.open:
                audioSource.PlayOneShot(open_btn);
                break;
            case AudioType.buy:
                audioSource.PlayOneShot(buy);
                break;
            case AudioType.diceRoll:
                audioSource.PlayOneShot(diceroll);
                break;
            case AudioType.fail:
                audioSource.PlayOneShot(fail);
                break;
            case AudioType.lose:
                audioSource.PlayOneShot(lose);
                break;
            case AudioType.move:
                audioSource.PlayOneShot(move);
                break;
            case AudioType.pay:
                audioSource.PlayOneShot(pay);
                break;
            case AudioType.success:
                audioSource.PlayOneShot(success);
                break;
        }
    }
}
