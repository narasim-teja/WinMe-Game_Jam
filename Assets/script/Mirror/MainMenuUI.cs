using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEditor;

public class MainMenuUI : MonoBehaviour
{
    NetworkManager manager;
    [SerializeField] private GameObject playerNameInputField;
    [SerializeField] private TMP_InputField IpInputField;
    [SerializeField] private TMP_InputField portInputField;
    private string playerName;

    // static Variable for Lobby System
    public static string LobbyAssignedserverIp = null;
    public static string LobbyAssignedserverPort = null;

    void Start() {
        #if UNITY_SERVER
            StartServerButtonClicked();
        // #else
        //     if(serverIp != null && serverPort != null){
        //         StartButtonClicked();
        //     }
        #endif
        
    }
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void StartButtonClicked()
    {
        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        // For Lobby
        if(LobbyAssignedserverIp != null) manager.networkAddress = LobbyAssignedserverIp;
        else manager.networkAddress = IpInputField.text.ToString() ;

        if (Transport.active is PortTransport portTransport)
        {
            //For Lobby
            string portInput;
            if(LobbyAssignedserverPort != null) portInput = LobbyAssignedserverPort ;
            else portInput = portInputField.text.ToString();
            
            if (ushort.TryParse(portInput, out ushort port))
            {
                portTransport.Port = port;
            }
        }

        manager.StartClient();
        StartCoroutine(WaitForLocalPlayerAndSetPlayerName());
    }

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

        PlayerManager localPlayer = NetworkClient.localPlayer.GetComponent<PlayerManager>();
        playerName = playerNameInputField.GetComponent<TMP_InputField>().text.ToString();
        
        localPlayer.SetPlayerName(playerName);
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
        }
    }
}
