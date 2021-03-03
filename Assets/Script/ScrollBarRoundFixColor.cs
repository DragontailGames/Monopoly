using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollBarRoundFixColor : MonoBehaviour
{
    Scrollbar scrollbar;

    Color lastColor;

    private void Start()
    {
        scrollbar = this.GetComponent<Scrollbar>();
    }

    private void Update()
    {
        Color newColor = scrollbar.handleRect.GetComponent<Image>().color;
        if (lastColor != null && lastColor != newColor)
        {
            foreach (Transform aux in scrollbar.handleRect.transform)
            {
                aux.GetComponent<Image>().color = newColor;
            }

            lastColor = newColor;
        }
    }
}
