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

        if (isLocalPlayer)
        {
            coinCountCanvas.gameObject.SetActive(true);
        }

        // Update the player name text when the client starts
        // OnPlayerNameChanged(playerName, playerName);
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
            if(isClient) CmdSetPlayerName(name);
            
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
        Transform firstChild = networkManager.transform.GetChild(0);
        firstChild.gameObject.SetActive(false);
    }
}
