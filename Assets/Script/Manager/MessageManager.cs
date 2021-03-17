using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    private static MessageManager _instance;

    private Color textColor;

    public static MessageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MessageManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public void ShowMessage(string text, bool hiddenAfterDelay = true)
    {
        StopCoroutine(DelayHiddenText());
        this.GetComponent<Image>().enabled = true;
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.GetComponentInChildren<TextMeshProUGUI>().text = text;
        if(hiddenAfterDelay)
            StartCoroutine(DelayHiddenText());
    }

    public IEnumerator DelayHiddenText()
    {
        yield return new WaitForSeconds(3.0f);
        HiddenText();
    }

    public void HiddenText()
    {
        this.GetComponent<Image>().enabled = false;
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool TextShowing()
    {
        return this.GetComponent<Image>().enabled;
    }
}
