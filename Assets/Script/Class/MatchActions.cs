using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MatchActions
{
    public UnityAction onConnectedToMaster;

    public UnityAction onJoinedLobby;

    public UnityAction onJoinedRoom;

    public UnityAction onLeftRoom;
}
