using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MirrorNetworkManager : NetworkManager
{
    public GameObject coinPrefab;
    public GameObject [] pickupList;

    [SerializeField]
    private int waitTimeForSceneChange = 5;

    private int playerCount = 0;
    public int noOfPlayers = 1;

    private readonly int waitForSecondsBeforeWhenNoPlayers = 60;
    private StoreData storeData;
    public static new MirrorNetworkManager singleton => NetworkManager.singleton as MirrorNetworkManager;

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
        if (conn.identity != null)
        {
            Debug.LogWarning("Connection already has a player");
            return;
        }


        GameObject player = Instantiate(StoreData.Instance.kartList[msg.kartIndex].obj,
            startPositions[startPositionIndex].position, Quaternion.identity);

        startPositionIndex = (startPositionIndex + 1) % startPositions.Count;


        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player spawned");

        StopCoroutine(StopServerWhenNoPlayers());

        playerCount++;
        if (playerCount == noOfPlayers)
        {
            StartCoroutine(WaitBeforeSceneStart());
        }
    }

    IEnumerator WaitBeforeSceneStart()
    {
        yield return new WaitForSeconds(waitTimeForSceneChange);
        LoadGameScene();
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

    // Reset position and velocity when scene change
    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);
        if(newSceneName == "MirrorCloverStadium")
        {
            foreach (var connection in NetworkServer.connections.Values)
            {
                if (connection.identity != null)
                {
                    GameObject playerObject = connection.identity.gameObject;
                    playerObject.transform.position = startPositions[startPositionIndex].position;

                    Rigidbody rb = playerObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }

                    startPositionIndex = (startPositionIndex + 1) % startPositions.Count;
                }
            }
        }else if(newSceneName == "MirrorWaitingRoom")
        {
            Debug.Log("Scene changed");
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client has stopped.");
    }
}
