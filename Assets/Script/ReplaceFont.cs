using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReplaceFont : MonoBehaviour
{
    public Font newFont;
    public TMP_FontAsset newFontTMP;

    [ContextMenu("Replace")]
    public void Replace()
    {
        foreach(TextMeshProUGUI txt in FindObjectsOfType<TextMeshProUGUI>())
        {
            txt.font = newFontTMP;
        }
        foreach (Text txt in FindObjectsOfType<Text>())
        {
            txt.font = newFont;
        }
    }
}
