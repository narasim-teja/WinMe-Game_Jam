// using UnityEngine;
// using Mirror;
// using TMPro;

// public class PlayerManager : NetworkBehaviour
// {
//     [SerializeField] private GameObject coinCountCanvas;

//     public string playerName = "";

//     public GameObject playerNameText;

//     private void Start()
//     {
//         DontDestroyOnLoad(gameObject);
//         // SetName();
//     }

//     public override void OnStartClient()
//     {
//         base.OnStartClient();
//         Debug.Log("Client started for player: " + netId);

//         DisableWaitingRoomCanvas();

//         if (isLocalPlayer)
//         {
//             coinCountCanvas.gameObject.SetActive(true);
//             // CmdSetPlayerName(playerName);
//         }
//     }

//     [Command]
//     private void CmdSetPlayerName(string name)
//     {
//         Debug.Log("2------:" + name);
//         playerNameText.GetComponent<TextMeshPro>().text = name;
//         // RpcUpdatePlayerName(name);
//     }

//     [ClientRpc]
//     public void RpcUpdatePlayerName(string name)
//     {
//         Debug.Log("3:" + name);
//         Debug.Log("___________");
//         playerName = name;
//         playerNameText.GetComponent<TextMeshPro>().text = playerName;
//     }

    
//     public void SetPlayerName(string name) 
//     {
//         Debug.Log("0:" + name);   
//         if (isLocalPlayer)
//         {
//             Debug.Log("1:" + name);
//             playerNameText.GetComponent<TextMeshPro>().text = name;
//         }
//     }

//     [Client]
//     private void DisableWaitingRoomCanvas()
//     {
//         GameObject networkManager = GameObject.Find("NetworkManager").gameObject;
//         Transform firstChild = networkManager.transform.GetChild(0);
//         firstChild.gameObject.SetActive(false);
//     }
// }


using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject coinCountCanvas;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    public GameObject playerNameText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started for player: " + netId);

        DisableWaitingRoomCanvas();
        AssembleKart();

        if (isLocalPlayer)
        {
            coinCountCanvas.gameObject.SetActive(true);
        }

        // Update the player name text when the client starts
        // OnPlayerNameChanged(playerName, playerName);
    }

    void AssembleKart()
    {
        Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.FR"));
        Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.FL"));

        Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.RR"));
        Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.RL"));

        Instantiate(StoreData.Instance.trailList[Constants.currentTrailIndex].obj, transform.Find("car/Wheel.RR").GetChild(0));
        Instantiate(StoreData.Instance.trailList[Constants.currentTrailIndex].obj, transform.Find("car/Wheel.RL").GetChild(0));
    }

    [Command]
    void CmdAssembleKart()
    {
        GameObject w1 = Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.FR"));
        GameObject w2 = Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.FL"));
        GameObject w3 = Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.RR"));
        GameObject w4 = Instantiate(StoreData.Instance.wheelList[Constants.currentWheelIndex].obj, transform.Find("car/Wheel.RL"));

        GameObject t1 = Instantiate(StoreData.Instance.trailList[Constants.currentTrailIndex].obj, transform.Find("car/Wheel.RR").GetChild(0));
        GameObject t2 = Instantiate(StoreData.Instance.trailList[Constants.currentTrailIndex].obj, transform.Find("car/Wheel.RL").GetChild(0));
        Debug.Log($"Assembling : {w1.name}");
        if (isServer)
        {
            Debug.Log("helllo");
            NetworkServer.Spawn(w1);
            NetworkServer.Spawn(w2);
            NetworkServer.Spawn(w3);
            NetworkServer.Spawn(w4);
            NetworkServer.Spawn(t1);
            NetworkServer.Spawn(t2);
            Debug.Log("helllo2222");
        }
    }


    [Command]
    private void CmdSetPlayerName(string name)
    {
        playerName = name; // This automatically updates the value on the clients because it's a SyncVar
        Debug.Log($"player name: {name}");
        if(playerNameText.GetComponent<TextMeshPro>() == null) Debug.Log("_player name__");
        playerNameText.GetComponent<TextMeshPro>().text = name;
        Debug.Log("Server updated player name to: " + name);
    }

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        Debug.Log("Player name changed from " + oldName + " to " + newName);
        playerNameText.GetComponent<TextMeshPro>().text = newName;
    }

    public void SetPlayerName(string name) 
    {
        if (isLocalPlayer)
        {
            Debug.Log("Setting player name: " + name);
            StartCoroutine(WaitForPlayerConnection());
            if(isClient)
            {
                CmdSetPlayerName(name);
                CmdAssembleKart();
            }

            
        }
    }
    private IEnumerator WaitForPlayerConnection()
    {
        while (!NetworkClient.isConnected)
        {
            yield return null;
        }
        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }
    }

    [Client]
    private void DisableWaitingRoomCanvas()
    {
        GameObject networkManager = GameObject.Find("NetworkManager").gameObject;
        Transform mainMenuCanvas = networkManager.transform.GetChild(0);

        Transform mainPanel = mainMenuCanvas.transform.GetChild(0);
        mainPanel.gameObject.SetActive(false);
        Transform lobbyPanel = mainMenuCanvas.transform.GetChild(2);
        lobbyPanel.gameObject.SetActive(true);
        //firstChild.gameObject.SetActive(false);
    }
}
