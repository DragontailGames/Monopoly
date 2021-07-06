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
        SceneManager.LoadScene(0);
    }
}