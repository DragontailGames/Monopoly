using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider slider;
    
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volume");
    }

    public void ChangeVolume(float volume)
    {
        SettingsManager.instance.ChangeVolume(volume);
    }
}
