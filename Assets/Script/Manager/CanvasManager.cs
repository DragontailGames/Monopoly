﻿using System.Collections;
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
        StartCoroutine(LoadSceneMenu());
    }

    private IEnumerator LoadSceneMenu()
    {
        yield return new WaitForSeconds(0.5f);

        var objects = FindObjectsOfType<GameObject>();

        Destroy(Manager.instance.gameObject);

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
    }
}