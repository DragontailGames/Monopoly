using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dragontailgames.Utils;

public class BaseManager : MonoBehaviour
{
    public Translation translation;

    public void Awake()
    {
        translation.afterTranslate = () => { SceneLoadManager.instance.gotoNext(); };
    }

}
