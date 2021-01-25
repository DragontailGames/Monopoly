using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Dragontailgames.Utils;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject popupWaitingPlayers;

    private TextMeshProUGUI txt_Waiting;

    private TextMeshProUGUI txt_PlayerCount;

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToMaster()
    {
        popupWaitingPlayers.SetActive(true);

        txt_Waiting = popupWaitingPlayers.transform.GetChild(0).Find("txt_Waiting").GetComponent<TextMeshProUGUI>();
        txt_PlayerCount = popupWaitingPlayers.transform.GetChild(0).Find("txt_PlayerCount").GetComponent<TextMeshProUGUI>();

        txt_Waiting.text = "Connecting...";
        txt_PlayerCount.text = "";


        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("We are connected already.");
        }
    }

    TypedLobby normalLobby = new TypedLobby("NormalPlayer", LobbyType.Default);
    TypedLobby botLobby = new TypedLobby("BotGame", LobbyType.Default);
    TypedLobby lobby;

    public void btn_ConnectToLobbyNormal()
    {
        ConnectToMaster();

        lobby = normalLobby;
    }

    public void btn_ConnectToLobbyBot()
    {
        ConnectToMaster();

        lobby = botLobby;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = "Player_" + Random.Range(0000, 9999);

        PhotonNetwork.JoinLobby(lobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=blue>Entrou no lobby</color>");
        JoinRoom();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.CountOfRooms > 0)
        {
            PhotonNetwork.JoinRandomRoom();
            return;
        }
        string name = "Monopoly_" + Random.Range(0000, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", 1 } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" }; // this makes "C0" available in the lobby
        PhotonNetwork.JoinOrCreateRoom(name, roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("<color=green>Entrou na sala " + PhotonNetwork.CurrentRoom.Name + "</color>");

        if (PhotonNetwork.CurrentLobby == botLobby)
        {
            GotoAdventurePhoton();
        }
        else
        {
            txt_Waiting.text = "Waiting for another players...";
            txt_PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2/*PhotonNetwork.CurrentRoom.MaxPlayers*/)
            {
                GotoAdventurePhoton();
            }
        }
    }

    public void btn_LeftRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        txt_PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2/*PhotonNetwork.CurrentRoom.MaxPlayers*/)
        {
            GotoAdventurePhoton();
        }
    }

    public void GotoAdventurePhoton()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    #region Fail Logs

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red>Erro ao tentar entrar em sala aleatoria " + message + "</color>");
        popupWaitingPlayers.SetActive(false);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red>Erro ao tentar entrar em sala " + message + "</color>");
        popupWaitingPlayers.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        popupWaitingPlayers.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        txt_PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #endregion
}
