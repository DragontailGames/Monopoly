using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using Photon.Pun.UtilityScripts;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Transform startTile;

    public GameManager manager;

    public Player[] GetPlayerList
    {
        get { return PhotonNetwork.PlayerList; }
    }

    public int GetPlayerNetworkCount
    {
        get { return PhotonNetwork.PlayerList.Length + Manager.instance.bots; }
    }

    public bool IsMaster
    {
        get { return PhotonNetwork.IsMasterClient; }
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    void Start()
    {
        CreatePlayer(startTile.position);

        if(PhotonNetwork.IsMasterClient)
        {
            var bots = Manager.instance.bots;
            var count = PhotonNetwork.PlayerList.Length + 1;
            while(bots>0)
            {
                CreatePlayer(startTile.position, true, count);
                bots--;
                count ++;
            }
        }
    }

    public void CreatePlayer(Vector3 position, bool isBot = false, int botNumber = 0)
    {
        ///NAO USAR O INSTATIATE
        ///https://forum.photonengine.com/discussion/16808/ai-bots-destroying-when-master-client-leaves-from-room
        Player player = null;
        position.y = 0.35f;
        GameObject playerGO = null;
        PlayerController playerController = null;

        if (isBot)
        {
            playerGO = PhotonNetwork.InstantiateRoomObject ("Prefabs/Player", position, Quaternion.Euler(new Vector3(0, 180, 0)));
            playerController = playerGO.GetComponent<PlayerController>();
            player = new Player(botNumber);
        }
        else
        {
            playerGO = PhotonNetwork.Instantiate("Prefabs/Player", position, Quaternion.Euler(new Vector3(0, 180, 0)));
            playerController = playerGO.GetComponent<PlayerController>();
            player = PhotonNetwork.LocalPlayer;
        }
        playerController.SetupStart(player, isBot, botNumber, isBot? Names.GetName() : "");

        //Player test = SaveAndLoad.instance.ConfigPlayer(SaveAndLoad.instance.PlayerFromJson((string)player.CustomProperties["Player"]));

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        var p = manager.players.Find(n => n.player == otherPlayer);
        p.LogMessagePlayer($"{p.name} declarou falência e não pode mais jogar!", true);

        p.photonView.RPC("DeclareBankruptcy_CMD", RpcTarget.All);
    }
}
