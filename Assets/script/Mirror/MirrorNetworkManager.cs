// using UnityEngine;
// using Mirror;
// using UnityEngine.SceneManagement;
// using TMPro;

// public class MirrorNetworkManager : NetworkManager
// {
//     private int playerCount = 0;
//     public int noOfPlayers = 1;

//     private void Start()
//     {
//         Transform firstChild = transform.GetChild(0);
//         firstChild.gameObject.SetActive(true);
//     }

//     public override void OnServerConnect(NetworkConnectionToClient conn)
//     {
//         base.OnServerConnect(conn);
//         Debug.Log("Client connected: " + conn.connectionId);
//     }

//     public override void OnServerReady(NetworkConnectionToClient conn)
//     {
//         base.OnServerReady(conn);
//         Debug.Log("Client is ready: " + conn.connectionId);
//     }

//     public override void OnServerAddPlayer(NetworkConnectionToClient conn)
//     {
//         Debug.Log("Entered OnServerAddPlayer");

//         // Check if the connection already has a player
        
//         Vector3 start = new Vector3(0, 40f, 0);
//         GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);
        
//         NetworkServer.AddPlayerForConnection(conn, player);
//         Debug.Log("Player spawned");

//         playerCount++;
//         if (playerCount == noOfPlayers)
//         {
//             LoadGameScene();
//         }
        
//     }

//     public override void OnServerDisconnect(NetworkConnectionToClient conn)
//     {
//         Debug.Log("OnServerDisconnect called");
//         Debug.Log(playerCount);
//         base.OnServerDisconnect(conn);

//         playerCount--;

//         if (playerCount == 0)
//         {
//             string newSceneName = "MirrorWaitingRoom"; // Replace with your scene name
//             Debug.Log("player Disconnected!!!");
//             ServerChangeScene(newSceneName);
//         }
//     }

//     [Server]
//     private void LoadGameScene()
//     {
//         string newSceneName = "MirrorCloverStadium"; // Replace with your scene name
//         ServerChangeScene(newSceneName);
//     }

//     public override void OnStopClient()
//     {
//         base.OnStopClient();
//         Debug.Log("Client has stopped.");
//     }
// }

using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MirrorNetworkManager : NetworkManager
{
    private int playerCount = 0;
    public int noOfPlayers = 1;

    private readonly int waitForSecondsBeforeWhenNoPlayers = 60;
    private StoreData storeData;

    public struct CreateKartMessage : NetworkMessage
    {
        public int kartIndex;
        public int wheelIndex;
        public int trailIndex;
        public int hatIndex;
    }

    private void Start()
    {
        storeData = StoreData.Instance;
        if(storeData == null)
        {
            Debug.Log("Add store manager in scene");
        }
        Transform firstChild = transform.GetChild(0);
        firstChild.gameObject.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateKartMessage>(OnCreateKart);
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        CreateKartMessage message = new CreateKartMessage{
            kartIndex = Constants.currentKartIndex,
            wheelIndex = Constants.currentWheelIndex,
            trailIndex = Constants.currentTrailIndex,
            hatIndex = Constants.currentHatIndex
        };
        NetworkClient.Send(message);
    }

    void OnCreateKart(NetworkConnectionToClient conn, CreateKartMessage msg)
    {

        //Debug.Log("Entered OnServerAddPlayer");

        // Check if the connection already has a player
        if (conn.identity != null)
        {
            Debug.LogWarning("Connection already has a player");
            return;
        }


        Vector3 start = new(0, 40f, 0);
        GameObject player = Instantiate(StoreData.Instance.kartList[msg.kartIndex].obj,
            start, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player spawned");

        StopCoroutine(StopServerWhenNoPlayers());

        playerCount++;
        if (playerCount == noOfPlayers)
        {
            LoadGameScene();
        }
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

    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    Debug.Log("Entered OnServerAddPlayer");

    //    // Check if the connection already has a player
    //    if (conn.identity != null)
    //    {
    //        Debug.LogWarning("Connection already has a player");
    //        return;
    //    }

    //    Vector3 start = new Vector3(0, 40f, 0);
    //    GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);

    //    NetworkServer.AddPlayerForConnection(conn, player);
    //    Debug.Log("Player spawned");

    //    StopCoroutine(StopServerWhenNoPlayers());
        
    //    playerCount++;
    //    if (playerCount == noOfPlayers)
    //    {
    //        LoadGameScene();
    //    }
    //}

    IEnumerator StopServerWhenNoPlayers()
    {
        yield return new WaitForSeconds(waitForSecondsBeforeWhenNoPlayers);

        if(playerCount == 0)
        {
            yield return new WaitUntil(()=> DeployApi.Instance.StopServerGracefully().IsCompleted);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerDisconnect called");
        base.OnServerDisconnect(conn);

        playerCount--;

        if (playerCount == 0)
        {
            StartCoroutine(StopServerWhenNoPlayers());
            string newSceneName = "MirrorWaitingRoom"; // Replace with your scene name
            Debug.Log("Player Disconnected!!!");
            ServerChangeScene(newSceneName);
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
