using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MainServer
{
    public int ServerID { get; set; }
    public string IP { get; set; }
    public string Port { get; set; }
    public string Status { get; set; }
    public List<string> PlayerIds { get; set; }
}
public class MirrorNetworkManager : NetworkManager
{
    private int playerCount = 0;
    public int noOfPlayers = 1;

    public List<MainServer> publicServerList;
    public List<string> privateServerList;
    NetworkManager manager;
    private void Start()
    {
        Transform firstChild = transform.GetChild(0);
        firstChild.gameObject.SetActive(true);

        manager = GetComponent<NetworkManager>();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client connected: " + conn.connectionId);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log("Client is ready: " + conn.connectionId);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Entered OnServerAddPlayer");
        // Check if the connection already has a player
        if (conn.identity != null)
        {
            Debug.LogWarning("Connection already has a player");
            return;
        }  

        // if(IsServerDataAssignedToClient(conn) == false){
            //create new server using API 
        // }
        // else{
        //     // AssignGameServerDataToClient(conn,);
        // }

        Vector3 start = new Vector3(0, 40f, 0);
        GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);
        
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player spawned");

        playerCount++;
        if (playerCount == noOfPlayers)
        {
            LoadGameScene();
        }
    }

    // [TargetRpc]
    // void AssignGameServerDataToClient(NetworkConnection target, string gameServerIP, string gameServerPort)
    // {
    //     MainMenuUI.LobbyAssignedserverIp = gameServerIP;
    //     MainMenuUI.LobbyAssignedserverPort = gameServerPort;
    //     StopClient();
    // }

    // [TargetRpc]
    // bool IsServerDataAssignedToClient(NetworkConnection target)
    // {
    //     if(MainMenuUI.LobbyAssignedserverIp != null && MainMenuUI.LobbyAssignedserverPort != null) return true;
    //     else return false;
    // }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerDisconnect called");
        base.OnServerDisconnect(conn);

        playerCount--;

        if (playerCount == 0)
        {
            string newSceneName = "MirrorWaitingRoom"; // Replace with your scene name
            Debug.Log("Player Disconnected!!!");
            ServerChangeScene(newSceneName);
        }
    }


    // For Lobby
    public override void OnClientDisconnect(){
        base.OnClientDisconnect();
        Debug.Log("Client disconnected");

        MainMenuUI mainMenuUIInstance = FindObjectOfType<MainMenuUI>();
        if (mainMenuUIInstance != null)
        {
            if(MainMenuUI.LobbyAssignedserverIp != null && MainMenuUI.LobbyAssignedserverPort != null){
                mainMenuUIInstance.StartButtonClicked();
            }
        }
        else
        {
            Debug.LogError("MainMenuUI not found!");
        }
    }


    [Server]
    private void LoadGameScene()
    {
        string newSceneName = "MirrorCloverStadium"; // Replace with your scene name
        ServerChangeScene(newSceneName);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client has stopped.");
    }
}
