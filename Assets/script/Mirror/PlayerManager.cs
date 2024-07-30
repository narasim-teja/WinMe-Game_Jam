using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject coinCountCanvas;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Add your client-specific initialization code here
        // Player.localPlayer.HostGame(false);
        GameObject.Find("NetworkManager").GetComponent<Player>().HostGame(false);
        Debug.Log("Client started for player: " + netId);

        // Example: Disabling the canvas
        DisableWaitingRoomCanvas();

        if (isLocalPlayer) coinCountCanvas.gameObject.SetActive(true);
    }

    [Client]
    private void DisableWaitingRoomCanvas()
    {
        GameObject networkManager = GameObject.Find("NetworkManager").gameObject;
        Transform firstChild = networkManager.transform.GetChild(0);
        firstChild.gameObject.SetActive(false);
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
