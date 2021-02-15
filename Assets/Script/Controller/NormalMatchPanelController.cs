using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dragontailgames.Utils;
using Photon.Pun;
using Photon.Realtime;

public class NormalMatchPanelController : MonoBehaviour
{
    //public SelectSpecialSkinController selectSpecialSkinController;

    //public UserManager user;

    //public Image diceIcon;

    //public TextMeshProUGUI txtBet;

    public TextMeshProUGUI roomName;

    public LobbyManager lobbyManager;

    public Button startGame;

    public Transform players;

    public List<string> bots;

    public void Start()
    {
        UpdatePlayers();
        lobbyManager.actions = new MatchActions()
        {
            onJoinedRoom = () =>
            {
                UpdatePlayers();
            },
            onPlayerEnteredRoom = () =>
            {
                UpdatePlayers();
            }
        };
        roomName.text = lobbyManager.roomName;
    }

    public void Update()
    {
        if(lobbyManager.GetCurrentRoomPlayers() + bots.Count>1)
        {
            startGame.interactable = true;
        }
        else
        {
            startGame.interactable = false;
        }
    }

    public GameObject blockPanel;

    public void StartGame()
    {
        blockPanel.SetActive(true);

        startGame.image.color = Color.cyan;
        startGame.GetComponentInChildren<TextMeshProUGUI>().text = "Starting...";
        startGame.interactable = false;
        lobbyManager.GotoAdventurePhoton();
    }

    public void UpdatePlayers()
    {
        for (int i = 0; i < lobbyManager.GetCurrentRoomPlayers(); i++)
        {
            foreach (Transform aux in players.GetChild(i))
            {
                aux.gameObject.SetActive(false);
            }
            GameObject normalPlayer = players.GetChild(i).GetChild(0).gameObject;
            normalPlayer.SetActive(true);
            normalPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobbyManager.GetPlayerNickname(i);
        }
        UpdateNewEntry();
    }

    public void AddBot()
    {
        this.GetComponent<PhotonView>().RPC("AddBotCMD", RpcTarget.All);
    }

    [PunRPC]
    public void AddBotCMD()
    {
        bots.Add("Bot_Rich" + Random.Range(0000, 9999));


        for (int i = 0; i < bots.Count; i++)
        {
            int index = i + lobbyManager.GetCurrentRoomPlayers();
            foreach (Transform aux in players.GetChild(index))
            {
                aux.gameObject.SetActive(false);
            }
            GameObject botPlayer = players.GetChild(index).GetChild(1).gameObject;
            botPlayer.SetActive(true);
            botPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bots[i];
        }
        UpdateNewEntry();
    }

    public void UpdateNewEntry()
    {
        if (lobbyManager.IsMaster)
        {
            int countExtra = 0;
            int newEntry = bots.Count + lobbyManager.GetCurrentRoomPlayers() + countExtra;

            if (bots.Count + lobbyManager.GetCurrentRoomPlayers() < 4)
            {
                players.GetChild(newEntry).GetChild(2).gameObject.SetActive(true);
            }

            countExtra++;
            newEntry = bots.Count + lobbyManager.GetCurrentRoomPlayers() + countExtra;

            Debug.Log("Teste " + newEntry + " - " + players.GetChild(newEntry).transform.name);

            while (players.GetChild(newEntry))
            {
                foreach (Transform aux in players.GetChild(newEntry))
                {
                    aux.gameObject.SetActive(false);
                }

                countExtra++;
                newEntry = bots.Count + lobbyManager.GetCurrentRoomPlayers() + countExtra;
            }            
        }
    }
}
