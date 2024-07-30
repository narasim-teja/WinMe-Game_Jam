using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenuUI : MonoBehaviour
{
    NetworkManager manager;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
        manager.networkAddress = "localhost";

        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse("localhost", out ushort port))
                portTransport.Port = port;
        }
    }

    public void startButtonClicked()
    {
        Debug.Log("Start Button Clicked");

        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        manager.StartClient();
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
                NetworkClient.AddPlayer();
        }

        Debug.Log("Client Started");
        Debug.Log(manager.networkAddress);
    }

    public void startServerButtonClicked()
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

            // Player.localPlayer.HostGame(false);
            // this.GetComponent<Player>().HostGame(false);

            Debug.Log("Server Started");
            if (Transport.active is PortTransport portTransport)
            {
                Debug.Log(portTransport.Port);
                Debug.Log(manager.networkAddress);
            }
        }
    }

    public void hostGame()
    {
        startButtonClicked();
        // this.GetComponent<Player>().HostGame(false);
    }
}
