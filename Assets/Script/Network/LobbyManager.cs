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

    public UserManager user;

    public MatchActions actions;

    [Tooltip("In Minutes per player in room (-1)")]
    public int autoBotJoinAfter = 120;

    private int fakeBots;

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToMaster()
    {
        //popupWaitingPlayers.SetActive(true);

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

    TypedLobby normalLobby = new TypedLobby("NormalGame", LobbyType.Default);
    TypedLobby botLobby = new TypedLobby("BotGame", LobbyType.Default);
    TypedLobby rankedLobby;
    TypedLobby lobby;

    public void btn_OpenMatchPanel(GameObject myObject)
    {
        myObject.transform.parent.gameObject.SetActive(true);
        myObject.SetActive(true);
    }

    public TMP_InputField roomNameInput;

    public void btn_ConnectToLobbyNormal()
    {
        this.roomName = roomNameInput.text;

        ConnectToMaster();

        lobby = normalLobby;
    }

    public void btn_ConnectToLobbyCustom()
    {
        ConnectToMaster();

        lobby = botLobby;
    }

    public void btn_ConnectToLobbyRanked(MatchActions matchActions, int bet)
    {
        rankedLobby = new TypedLobby("RankedGame_"+bet, LobbyType.Default);
        actions = matchActions;

        lobby = rankedLobby;

        ConnectToMaster();
    }

    public override void OnConnectedToMaster()
    {
        actions?.onConnectedToMaster?.Invoke();
        PhotonNetwork.NickName = user.nickname;

        PhotonNetwork.JoinLobby(lobby);
    }

    public override void OnJoinedLobby()
    {
        actions?.onJoinedLobby?.Invoke();
        Debug.Log("<color=blue>Entrou no lobby " + PhotonNetwork.CurrentLobby.Name + ".</color>");
        JoinRoom(roomName);
    }


    public void JoinRoom(bool force = false)
    {
        if (PhotonNetwork.CountOfRooms > 0 && !force)
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

    public string roomName;

    public void JoinRoom(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            JoinRoom();
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", 1 } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" }; // this makes "C0" available in the lobby
        PhotonNetwork.JoinOrCreateRoom(name, roomOptions, null);
    }

    IEnumerator botAutoJoin;

    public override void OnJoinedRoom()
    {
        actions?.onJoinedRoom?.Invoke();

        roomName = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("<color=green>Entrou na sala " + PhotonNetwork.CurrentRoom.Name + "</color>");

        if(IsMaster)
        {
            botAutoJoin = BotAutoJoin();
            StartCoroutine(botAutoJoin);
        }

        if (PhotonNetwork.CurrentLobby == botLobby)
        {
            GotoAdventurePhoton();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                GotoAdventurePhoton();
            }
        }
    }

    public void btn_LeftRoom()
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        actions?.onLeftRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        actions?.onPlayerEnteredRoom?.Invoke();
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount + fakeBots == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GotoAdventurePhoton();
        }
        else
        {
            StopCoroutine(botAutoJoin);
            StartCoroutine(botAutoJoin);
        }
    }

    public int GetCurrentRoomPlayers()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public string GetPlayerNickname(int index)
    {
        return PhotonNetwork.PlayerList[index].NickName;
    }

    public bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }

    public void GotoAdventurePhoton()
    {
        PlayerPrefs.SetInt("Bots", fakeBots);

        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    public IEnumerator BotAutoJoin()
    {
        yield return new WaitForSeconds(autoBotJoinAfter * (GetCurrentRoomPlayers() + fakeBots));
        if (PhotonNetwork.CurrentRoom.PlayerCount + fakeBots < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("Novo bot na sala");
            fakeBots++;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount + fakeBots < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            botAutoJoin = BotAutoJoin();
            StartCoroutine(botAutoJoin);
        }
        else
        {
            GotoAdventurePhoton();
        }
    }

    #region Fail Logs

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red>Erro ao tentar entrar em sala aleatoria " + message + "</color>");
        JoinRoom(true);
        //popupWaitingPlayers.SetActive(false);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red>Erro ao tentar entrar em sala " + message + "</color>");
        popupWaitingPlayers.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        //popupWaitingPlayers.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //txt_PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #endregion
}
