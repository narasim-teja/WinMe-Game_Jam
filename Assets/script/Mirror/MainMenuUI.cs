using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using System.Net;
using System.Linq;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Mirror.SimpleWeb;
using UnityEngine.SceneManagement;
using Thirdweb;
using UnityEngine.Networking;
using Org.BouncyCastle.Asn1.Ocsp;

public class MainMenuUI : MonoBehaviour
{
    public Camera main_camera;
    public GameObject main_menu_panel;
    public GameObject treasure_box_panel;
    public GameObject PopUpPrefab;

    // treasure box
    public GameObject treasure_box_env_prefab;
    private GameObject treasure_box_env_instance;
    // public Camera treasure_box_camera;
    // public ParticleSystem confeti_effect1;
    // public ParticleSystem confeti_effect2;
    private const string SERVER_IP = "serverIP";
    private const string SERVER_PORT = "serverPort";

    NetworkManager manager;
    SimpleWebTransport transport;

    [SerializeField] private GameObject playerNameInputField;
    private string playerName;
    private string defaultPlayerName;


    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;
    [SerializeField] private TMP_Text lobbyCodeText;
    [SerializeField] private GameObject lobbyCodeInputField;
    private string lobbyCode;
    private int maxPlayerInLobby = 3;

    private float heartbeatTimer;
    private ServerData serverData;

    [SerializeField]
    GameObject loadingPanel;
    private bool isLoading = false;
    

    void Start()
    {
        #if UNITY_SERVER
                StartServerButtonClicked();
        #endif
    }
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
        transport = GetComponent<SimpleWebTransport>();

        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse("localhost", out ushort port))
                portTransport.Port = port;
        }

        InitializeUnityAuthentication();

        defaultPlayerName = "Player"+ UnityEngine.Random.Range(1000, 10000).ToString();

    }

    private  void Update()
    {
        HandleHeartbeat();
    }

    public async void ValidateWalletAndMintNFT()
    {
        string walletAddress = string.Empty;

        try
        {
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error retrieving wallet address: {ex.Message}");
        }

        if (!string.IsNullOrEmpty(walletAddress))
        {
            bool token = await ThirdwebManager.Instance.SDK.Wallet.IsConnected();
            string data = await ThirdwebManager.Instance.SDK.Wallet.Sign("5fe69c95ed70a9869d9f9ayush27f7d8400a6673bb9ce9");

            StartCoroutine(ServerRequestToMintNFT(walletAddress,data));
        }
    }
    [System.Serializable]
    public class AuthPayload
    {
        public string walletAddress;
        public string encryptedmessage;
}
    private IEnumerator ServerRequestToMintNFT(string walletAddress, string message)
    {
        string payload = JsonUtility.ToJson(new AuthPayload
        {
            walletAddress = walletAddress,
            encryptedmessage = message
        });

        Debug.Log("Payload: " + payload);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3001/mintItem", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                Debug.Log("heartbeat sent");
            }
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Guid.NewGuid().ToString()[..8]);
            //initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public void StartButtonClicked()
    {
        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        Debug.Log(manager.networkAddress);

        manager.StartClient();
        StartCoroutine(WaitForLocalPlayerAndSetPlayerName());
    }

    private void StartGame(string serverIP, string serverPort)
    {

        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        manager.networkAddress = serverIP;
        transport.port = ushort.Parse(serverPort);

        manager.StartClient();
        StartCoroutine(WaitForLocalPlayerAndSetPlayerName());
    }


    #region Server fucntions
    private IEnumerator WaitForLocalPlayerAndSetPlayerName()
    {
        while (NetworkClient.localPlayer == null)   
        {
            Debug.Log("Waiting for local player...");
            yield return null;
        }
        GameObject.Find("Camera").SetActive(false);

        PlayerManager localPlayer = NetworkClient.localPlayer.GetComponent<PlayerManager>();
        playerName = playerNameInputField.GetComponent<TMP_InputField>().text.ToString();


        if(playerName == "") playerName = defaultPlayerName;
        localPlayer.SetPlayerInfo(new()
        {
            name = playerName,
            wheelIndex = Constants.currentWheelIndex,
            trailIndex = Constants.currentTrailIndex,
            hatIndex = Constants.currentHatIndex,
        });

    }

    public void StartServerButtonClicked()
    {
        if (!NetworkClient.active)
        {
            Debug.Log("Start Server Button Clicked");

            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }

            manager.StartServer();

            Debug.Log("Server Started");
            if (Transport.active is PortTransport portTransport)
            {
                Debug.Log(portTransport.Port);
            }

            Debug.Log("recent new file");

        }

    }
    #endregion

    #region Lobby Functions

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void QuickJoinPressed()
    {
        if (isLoading) return;
        LoadingStart();

        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string serverIP = joinedLobby.Data[SERVER_IP].Value;
            string serverPort = joinedLobby.Data[SERVER_PORT].Value;

            StartGame(serverIP, serverPort);
            LoadingEnd();
            //Debug.Log($"joined lobby,lobby name: {joinedLobby.Name}, lobby id: {joinedLobby.Id}, serverip: {serverIP}, serverport: {serverPort}");
        }
        catch (LobbyServiceException e)
        {
            //Debug.Log(e);
            if(e.Reason == LobbyExceptionReason.NoOpenLobbies)
            {
                //Debug.Log("no open lobbies");
                serverData = await DeployApi.Instance.CreateNewServer();
                if (serverData == null || !serverData.isReady)
                {
                    Debug.Log("server creation failed.");
                    LoadingEnd();
                }
                else
                {
                    CreateLobby("first Lobby", false, maxPlayerInLobby,serverData.ip, serverData.port);
                }
                //CreateLobby("first Lobby", false, 2, "abcd", "port 222");
            }
            else
            {
                LoadingEnd();
                Debug.Log(e);
            }
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void UpdateCurLobbyDetails()
    {
        lobbyCodeText.text = $"Code: {joinedLobby.LobbyCode}";
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter> {
                  new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
            Debug.Log($"lobbies number: {queryResponse.Results.Count}");
            foreach (var lob in queryResponse.Results)
            {
                Debug.Log($"id: {lob.Id}, name: {lob.Name}, host id: {lob.HostId}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreatePrivateLobby()
    {
        if (isLoading) return;

        LoadingStart();

        serverData = await DeployApi.Instance.CreateNewServer();
        if (serverData == null || !serverData.isReady)
        {
            Debug.Log("server creation failed.");
            LoadingEnd();
        }
        else
        {
            CreateLobby("first Lobby", true, maxPlayerInLobby, serverData.ip, serverData.port);
        }
    }

    public async void JoinLobby()
    {
        if (isLoading) return;

        LoadingStart();

        lobbyCode = lobbyCodeInputField.GetComponent<TMP_InputField>().text.ToString();
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string serverIP = joinedLobby.Data[SERVER_IP].Value;
            string serverPort = joinedLobby.Data[SERVER_PORT].Value;

            StartGame(serverIP, serverPort);
            LoadingEnd();

            Debug.Log($"lobby code: {joinedLobby.LobbyCode}, lobby name: {joinedLobby.Name}, lobby id: {joinedLobby.Id}, serverip: {serverIP}, serverport: {serverPort}");
        }
        catch(LobbyServiceException e)
        {
            lobbyCode = null;
            LoadingEnd();
            Debug.Log(e.Message);
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate, int maxPlayers, string serverIP, string serverPort)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        SERVER_IP, new DataObject(
                            visibility: DataObject.VisibilityOptions.Member, // Visible publicly.
                            value: serverIP)
                    },{
                        SERVER_PORT, new DataObject(
                            visibility: DataObject.VisibilityOptions.Member, // Visible publicly.
                            value: serverPort)
                    },
                }
            });

            UpdateCurLobbyDetails();
            StartGame(serverIP, serverPort);
            LoadingEnd();

            Debug.Log($"id: {joinedLobby.Id}, name: {joinedLobby.Name}, host id: {joinedLobby.HostId}, lobby code: {joinedLobby.LobbyCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            joinedLobby = null;
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    #endregion



    #region Loading region
    private void LoadingStart()
    {
        loadingPanel.SetActive(true);
        isLoading = true;
    }

    private void LoadingEnd()
    {
        loadingPanel.SetActive(false);
        isLoading= false;
    }
    #endregion


    #region Go to store
    public void LoadStoreScene()
    {
        // if(!ThirdwebManager.Instance.SDK.Wallet.IsConnected().Result){
        //     GameObject PopUpInstance = Instantiate(PopUpPrefab);
        //     PopUpInstance.GetComponent<PopUpMessage>().UpdatePopUpMessage("Connect your wallet first to go to the shop.");
        //     return;
        // }
        // disabling component of network manager
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        SceneManager.LoadScene(2);
    }
    #endregion

    #region treasure box
    public void OpenTreasureBoxUIButton()
    {
        // if(!ThirdwebManager.Instance.SDK.Wallet.IsConnected().Result){
        //     GameObject PopUpInstance = Instantiate(PopUpPrefab);
        //     PopUpInstance.GetComponent<PopUpMessage>().UpdatePopUpMessage("Connect your wallet first to unlock the treasure box.");
        //     return;
        // }

        StartCoroutine(getRandomItemFromServer());

        main_camera.gameObject.SetActive(false);
        main_menu_panel.gameObject.SetActive(false);
        treasure_box_panel.gameObject.SetActive(true);

    }

    [System.Serializable]
    public class TreasureBoxItemServerResponse
    {
        public string type;
        public Item item;

        [System.Serializable]
        public class Item
        {
            public string title;
            public string rarity;
            public float probability; 
        }
    }
    private IEnumerator getRandomItemFromServer(){ 
        UnityWebRequest request = new UnityWebRequest("http://localhost:3001/random-item", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Server Response: " + jsonResponse);

            TreasureBoxItemServerResponse serverResponse = JsonUtility.FromJson<TreasureBoxItemServerResponse>(jsonResponse);
            Debug.Log("Type: " + serverResponse.type);
            Debug.Log("Item Name: " + serverResponse.item.title);

            StoreItem[] allItems = Resources.LoadAll<StoreItem>("ScriptableObject/"+serverResponse.type);
            foreach (StoreItem item in allItems)
            {
                print(serverResponse.item.title+"---" + item.title);

                if(serverResponse.item.title == item.title){
                    if(treasure_box_env_instance == null) {
                        treasure_box_env_instance = Instantiate(treasure_box_env_prefab);
                        treasure_box_env_instance.GetComponent<treasure_box_script>().ObtainedItem = item.obj;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
    public void ClaimTreasureBoxButton()
    {
        ValidateWalletAndMintNFT();
        Destroy(treasure_box_env_instance);
        main_camera.gameObject.SetActive(true);
        main_menu_panel.gameObject.SetActive(true);
        treasure_box_panel.gameObject.SetActive(false);
    }
    public void CloseTreasureBoxButton()
    {
        Destroy(treasure_box_env_instance);
        main_camera.gameObject.SetActive(true);
        main_menu_panel.gameObject.SetActive(true);
        treasure_box_panel.gameObject.SetActive(false);
    }
    #endregion
}
