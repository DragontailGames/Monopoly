using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Dragontailgames.Utils;
using TMPro;

public class ExportToCsv : MonoBehaviour
{
    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach (var aux in FindObjectsOfType(typeof(TextMeshProUGUI)))
        {
            var temp = aux as TextMeshProUGUI;
            DestroyImmediate(temp.GetComponent<TranslationText>());
        }
    }

    [ContextMenu("SetupTranslation")]
    public void SetupTranslation()
    {
        foreach (var aux in FindObjectsOfType(typeof(TextMeshProUGUI)))
        {
            var temp = aux as TextMeshProUGUI;
            if (!temp.GetComponent<TranslationText>())
            {
                var tempTT = temp.gameObject.AddComponent(typeof(TranslationText)) as TranslationText;
                tempTT.tagId = temp.text;
            }
            else
            {
                temp.GetComponent<TranslationText>().tagId = temp.text;
            }
        }
    }

    [ContextMenu("Export")]
    public void Export()
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"Tags.txt"))
        {
            foreach (var aux in FindObjectsOfType(typeof(TranslationText)))
            {
                var temp = aux as TranslationText;
                file.WriteLine(temp.tagId);
            }

            file.Close();
        }
    }
}
