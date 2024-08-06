using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;

[System.Serializable]
public class GameMatch : NetworkBehaviour
{
    public string matchID;
    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;
    public List<Player> players = new();

    public GameMatch(string matchID, Player player, bool publicMatch)
    {
        matchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.publicMatch = publicMatch;
        players.Add(player);
    }

    public GameMatch() { }
}

public class MatchMaker : NetworkBehaviour
{
    // public static MatchMaker Instance { get; private set; }

    // public List<GameMatch> matches = new();
    public List<string> matchIDs = new();

    // [SerializeField] int maxMatchPlayers = 12;

    void Start()
    {
        Debug.Log("helloasdfdsgs");
        Swaaaaan();
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    public void Swaaaaan()
    {
        // NetworkServer.Spawn(gameObject);
        CreateLobby();
    }

    [Command]
    public void CreateLobby()
    {
        string code = GetRandomMatchID();
        matchIDs.Add(code);
        Debug.Log(matchIDs.Count);
    }

    /*
    public bool HostGame(string _matchID, Player _player, bool publicMatch, out int playerIndex)
    {
        playerIndex = -1;

        if (!matchIDs.Contains(_matchID))
        {
            matchIDs.Add(_matchID);
            GameMatch match = new(_matchID, _player, publicMatch);
            matches.Add(match);
            Debug.Log($"GameMatch generated");
            // _player.currentMatch = match;
            // playerIndex = 1;
            return true;
        }
        else
        {
            Debug.Log($"GameMatch ID already exists");
            return false;
        }
    }

    public bool JoinGame(string _matchID, Player _player, out int playerIndex)
    {
        playerIndex = -1;

        if (matchIDs.Contains(_matchID))
        {

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    if (!matches[i].inMatch && !matches[i].matchFull)
                    {
                        matches[i].players.Add(_player);
                        // _player.currentMatch = matches[i];
                        // playerIndex = matches[i].players.Count;

                        // matches[i].players[0].PlayerCountUpdated(matches[i].players.Count);

                        // if (matches[i].players.Count == maxMatchPlayers)
                        // {
                        //     matches[i].matchFull = true;
                        // }

                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            Debug.Log($"GameMatch joined");
            return true;
        }
        else
        {
            Debug.Log($"GameMatch ID does not exist");
            return false;
        }
    }
    */

    public static string GetRandomMatchID()
    {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
            else
            {
                _id += (random - 26).ToString();
            }
        }
        Debug.Log($"Random GameMatch ID: {_id}");
        return _id;
    }

}



public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
