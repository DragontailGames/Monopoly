using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public GameManager gameManager;

    public Button btnThrow;

    public BuyableMenuController buyableMenu;

    public JailMenuController jailMenuController;

    public GameObject[] playerCanvas;

    public GameObject endOfGame;

    public void GotoMenu()
    {
        gameManager.networkManager.Leave();
        Invoke("LoadSceneMenu", 1.0f);
    }

    private void LoadSceneMenu()
    {
        SceneManager.LoadScene(0);
    }
}