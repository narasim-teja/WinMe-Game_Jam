using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class PlayerInfo
{
    public string name;
    public int wheelIndex = -1;
    public int trailIndex = -1;
}

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject coinCountCanvas;


    [SyncVar(hook = nameof(OnPlayerInfoChanged))]
    public PlayerInfo playerInfo;

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
        //AssembleKart();

        if (isLocalPlayer)
        {
            coinCountCanvas.gameObject.SetActive(true);
        }

        // Update the player name text when the client starts
        // OnPlayerNameChanged(playerName, playerName);
    }

    [Command]
    void CmdAssemblePlayer(PlayerInfo cur)
    {
        playerInfo = cur;  // This automatically updates the value on the clients because it's a SyncVar
        Debug.Log($"player name: {cur.name}");
        if (playerNameText.GetComponent<TextMeshPro>() == null) Debug.Log("_player name__");
        playerNameText.GetComponent<TextMeshPro>().text = cur.name;

        Debug.Log("Server updated player name to: " + name);
        Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.FR"));
        Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.FL"));
        Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.RR"));
        Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.RL"));

        Instantiate(StoreData.Instance.trailList[cur.trailIndex].obj, transform.Find("car/Wheel.RR").GetChild(0));
        Instantiate(StoreData.Instance.trailList[cur.trailIndex].obj, transform.Find("car/Wheel.RL").GetChild(0));
    }

    void OnPlayerInfoChanged(PlayerInfo old, PlayerInfo cur)
    {
        playerNameText.GetComponent<TextMeshPro>().text = cur.name;
        if (transform.Find("car/Wheel.FR").childCount == 0)
        {
            Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.FR"));
            Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.FL"));
            Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.RR"));
            Instantiate(StoreData.Instance.wheelList[cur.wheelIndex].obj, transform.Find("car/Wheel.RL"));

            Instantiate(StoreData.Instance.trailList[cur.trailIndex].obj, transform.Find("car/Wheel.RR").GetChild(0));
            Instantiate(StoreData.Instance.trailList[cur.trailIndex].obj, transform.Find("car/Wheel.RL").GetChild(0));
        }
    }


    public void SetPlayerInfo(PlayerInfo playerInfo) 
    {
        if (isLocalPlayer)
        {
            Debug.Log("Setting player name: " + name);
            StartCoroutine(WaitForPlayerConnection());
            if(isClient)
            {
                CmdAssemblePlayer(playerInfo);
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
