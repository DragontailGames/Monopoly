using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dragontailgames.Utils;

public class MatchManager : MonoBehaviour
{
    public string offlineScene;

    public string playerNormalScene;

    public string playerRankedScene;

    public void btn_OfflineLoad()
    {
        SceneLoadManager.instance.LoadScene(offlineScene);
    }
}
