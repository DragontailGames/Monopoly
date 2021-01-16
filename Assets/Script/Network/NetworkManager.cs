using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Transform startTile;

    public Player[] GetPlayerList
    {
        get { return PhotonNetwork.PlayerList; }
    }

    public int GetPlayerNetworkCount
    {
        get { return PhotonNetwork.PlayerList.Length; }
    }

    public bool IsMaster
    {
        get { return PhotonNetwork.IsMasterClient; }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                CreatePlayer(startTile.position);
            }
        }
    }

    public void CreatePlayer(Vector3 position)
    {

        Player player = PhotonNetwork.LocalPlayer;
        position.y = 0.35f;
        GameObject playerGO = PhotonNetwork.Instantiate("Prefabs/Player", position, Quaternion.identity);
        PlayerController playerController = playerGO.GetComponent<PlayerController>();

        playerController.SetupStart(player);

        //Player test = SaveAndLoad.instance.ConfigPlayer(SaveAndLoad.instance.PlayerFromJson((string)player.CustomProperties["Player"]));
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Index", player.ActorNumber } });

    }
}
