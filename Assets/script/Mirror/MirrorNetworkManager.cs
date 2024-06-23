using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MirrorNetworkManager : Mirror.NetworkManager
{
    //public Transform spawnLocation;
    private int playerCount = 0; // Moved the playerCount here for integration
    public int noOfPlayers = 1;

    private void Start()
    {
        Transform firstChild = transform.GetChild(0);
        firstChild.gameObject.SetActive(true);
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
        //Transform start = spawnLocation;
        Vector3 start = new Vector3(0,10f,0);
        GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);
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
        Debug.Log(playerCount);
        base.OnServerDisconnect(conn);

        // Decrement player count
        playerCount--;

        if(playerCount == 0)
        {

            string newSceneName = "MirrorWaitingRoom"; // Replace with your scene name
            Debug.Log("player Disconnected!!!");
            ServerChangeScene(newSceneName);
        } 
    }

    [Server]
    private void LoadGameScene()
    {
        /*if (transform.childCount > 0) // Ensure there is at least one child
        {
            Transform firstChild = transform.GetChild(0);
            if (firstChild != null)
            {
                firstChild.gameObject.SetActive(false);
                Debug.Log("First child deactivated: " + firstChild.gameObject.name);
            }
            else
            {
                Debug.LogWarning("First child transform is null.");
            }
        }
        else
        {
            Debug.LogWarning("No children to deactivate.");
        }*/
        // Make sure to add the scene you want to load to the build settings
        string newSceneName = "MirrorCloverStadium"; // Replace with your scene name
        ServerChangeScene(newSceneName);
    }

    /*public void DisconnectOnGameOver()
    {
        Debug.Log("111111111");
        if (isNetworkActive)
        {
            Debug.Log("222222222222");
            //if (NetworkClient.isConnected)
            //{
                Debug.Log("3333333333");
                StopClient();
            //}
        }
    }*/

    public override void OnStopClient()
    {
        base.OnStopClient(); 
        Debug.Log("Client has stopped.");
        //SceneManager.LoadScene("MirrorWaitingRoom");
    }

}
