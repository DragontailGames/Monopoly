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

    public Player[] GetPlayerList
    {
        get { return PhotonNetwork.PlayerList; }
    }

    public int GetPlayerNetworkCount
    {
        get { return PhotonNetwork.PlayerList.Length + PlayerPrefs.GetInt("Bots"); }
    }

    public bool IsMaster
    {
        get { return PhotonNetwork.IsMasterClient; }
    }

    void Start()
    {
        CreatePlayer(startTile.position);

        if(PhotonNetwork.IsMasterClient)
        {
            var bots = PlayerPrefs.GetInt("Bots");
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
        Player player = null;
        position.y = 0.35f;
        GameObject playerGO = PhotonNetwork.Instantiate("Prefabs/Player", position, Quaternion.Euler(new Vector3(0,180,0)));
        PlayerController playerController = playerGO.GetComponent<PlayerController>();

        if (isBot)
        {
            playerGO.AddComponent<BotController>();
            player = new Player(botNumber);
        }
        else
        {
            player = PhotonNetwork.LocalPlayer;
        }
        playerController.SetupStart(player, isBot, botNumber);

        //Player test = SaveAndLoad.instance.ConfigPlayer(SaveAndLoad.instance.PlayerFromJson((string)player.CustomProperties["Player"]));

    }
}
