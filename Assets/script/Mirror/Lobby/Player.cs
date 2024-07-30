using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    NetworkMatch networkMatch;

    [SyncVar] public GameMatch currentMatch;


    void Awake()
    {
        networkMatch = GetComponent<NetworkMatch>();
    }

    // Host game
    public void HostGame(bool publicMatch)
    {
        string matchID = MatchMaker.GetRandomMatchID();
        Debug.Log("sfsaf");
        CmdHostGame(matchID, publicMatch);
        Debug.Log("sfsaf222");
    }

    [Command]
    void CmdHostGame(string _matchID, bool publicMatch)
    {
        Debug.Log("dsdf");
        matchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, this, publicMatch, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game hosted failed</color>");
            TargetHostGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        // UILobby.instance.HostSuccess(success, _matchID);
    }

    // Join game
    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, this, out playerIndex))
        {
            Debug.Log($"<color=green>Game Joined successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, playerIndex);

            // //Host
            // if (isServer && playerLobbyUI != null)
            // {
            //     playerLobbyUI.SetActive(true);
            // }
        }
        else
        {
            Debug.Log($"<color=red>Game Joined failed</color>");
            TargetJoinGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        // UILobby.instance.JoinSuccess(success, _matchID);
    }

}