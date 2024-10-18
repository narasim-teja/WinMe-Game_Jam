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

public class MainMenuUI : MonoBehaviour
{
    private const string SERVER_IP = "serverIP";
    private const string SERVER_PORT = "serverPort";

    NetworkManager manager;
    SimpleWebTransport transport;

    [SerializeField]
    GameObject canvasCamera;

    [SerializeField] private GameObject playerNameInputField;
    private string playerName;


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

    [SerializeField]
    test testing;

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

        //InitializeUnityAuthentication();

    }

    //private void Update()
    //{
    //    HandleHeartbeat();
    //}


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
        
        // while (!NetworkClient.ready)
        // {
        //     Debug.Log("Waiting for local readyyy...");
        //     yield return null;
        // }
        // if (!NetworkClient.ready)
        // {
        //     // NetworkClient.Ready();
        // }
        // if (NetworkClient.localPlayer == null )
        // {
        //     Debug.Log("Attempting to add player");

        //     // NetworkClient.AddPlayer();
        // }
        while (NetworkClient.localPlayer == null)   
        {
            Debug.Log("Waiting for local player...");
            yield return null;
        }
        canvasCamera.SetActive(false);

        PlayerManager localPlayer = NetworkClient.localPlayer.GetComponent<PlayerManager>();
        playerName = playerNameInputField.GetComponent<TMP_InputField>().text.ToString();
        

        localPlayer.SetPlayerName(playerName);
        Debug.Log($"Request id old: {DeployApi.Instance.GetRequestId()}");
        DeployApi.Instance.SetRequestID("e28bab1a670f");
        Debug.Log($"Request id new: {DeployApi.Instance.GetRequestId()}");

        localPlayer.SetRequestID(DeployApi.Instance.GetRequestId());
    }

    public async void StartServerButtonClicked()
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
            //List<string> ls = new List<string> { "223.185.130.199" };
            ////await DeployApi.Instance.DeployServer(ls);
            //await DeployApi.Instance.DeployServerHttp(ls);
            Debug.Log("kdkakd");
            string ip = await DeployApi.Instance.GetPublicIp();
            Debug.Log($"{ip}");
            Debug.Log("ads");

            //string ip = await testing.GetPublicIp();
            //Debug.Log($"{ip}");
        }
    }
    #endregion

    #region Lobby Functions

    public async void Test()
    {
        DeployApi.Instance.SetRequestID("60172edd1814");
        await DeployApi.Instance.StopServer();
    }
    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void QuickJoinPressed()
    {
        if (isLoading) return;
        LoadingStart();

        OnJoinStarted?.Invoke(this, EventArgs.Empty);
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
                    CreateLobby("first Lobby", false, 2,serverData.ip, serverData.port);
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
            CreateLobby("first Lobby", true, 2, serverData.ip, serverData.port);
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


    public string GetLocalIPv4()
    {
        string strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();
    }
}
