using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    /*private static int playerCount = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkManager.singleton.onServerConnect += OnServerConnect;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkManager.singleton.onServerConnect -= OnServerConnect;
    }

    [Server]
    private void OnServerConnect(NetworkConnection conn)
    {
        playerCount++;
        Debug.Log("Player connected. Total players: " + playerCount);

        if (playerCount == 2)
        {
            LoadGameScene();
        }
    }

    [Server]
    private void LoadGameScene()
    {
        // Make sure to add the scene you want to load to the build settings
        string newSceneName = "GameScene"; // Replace with your scene name
        NetworkManager.singleton.ServerChangeScene(newSceneName);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        playerCount--;
        Debug.Log("Player disconnected. Total players: " + playerCount);
    }*/
}