using UnityEngine;
using Mirror;


public class MirrorNetworkManager : NetworkManager
{
    public Transform spawnLocation;
    private int playerCount = 0; // Moved the playerCount here for integration
    public int noOfPlayers = 1;
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
        Transform start = spawnLocation;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player spawned");

        // Increment player count and check if it is 2
        playerCount++;
        if (playerCount == noOfPlayers)
        {
            LoadGameScene();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerDisconnect called");
        base.OnServerDisconnect(conn);

        // Decrement player count
        playerCount--;
    }

    [Server]
    private void LoadGameScene()
    {
        // Make sure to add the scene you want to load to the build settings
        string newSceneName = "MirrorCloverStadium"; // Replace with your scene name
        ServerChangeScene(newSceneName);
    }
}
