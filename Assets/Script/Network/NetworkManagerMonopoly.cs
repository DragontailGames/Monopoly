using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Custom NetworkManager that simply assigns the correct racket positions when
// spawning players. The built in RoundRobin spawn method wouldn't work after
// someone reconnects (both players would be on the same side).
[AddComponentMenu("")]
public class NetworkManagerMonopoly : NetworkManager
{
    public Transform startTile;

    public GameManager manager;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Vector3 pos = startTile.position;
        pos.y = 0.3f;
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.currentTile = startTile.GetComponent<TileController>();
        StartCoroutine(playerController.moveController.RepositionInTile(playerController.playerNumber, numPlayers));

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
