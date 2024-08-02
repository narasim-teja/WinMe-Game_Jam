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
        StartCoroutine(WaitForLocalPlayer());
        // if (NetworkClient.isConnected && !NetworkClient.ready)
        // {
        //     NetworkClient.Ready();
        //     if (NetworkClient.localPlayer == null)
        //         NetworkClient.AddPlayer();
        // }

        // Debug.Log("Client Started");
        // Debug.Log(manager.networkAddress);
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

    // public void StartButtonClicked()
    // {
    //     if (manager == null)
    //     {
    //         Debug.LogError("NetworkManager not found");
    //         return;
    //     }

    //     manager.StartClient();
    //     StartCoroutine(WaitForLocalPlayer());
    // }

    private IEnumerator WaitForLocalPlayer()
    {


        while (!NetworkClient.isConnected)
        {
            yield return null;
        }
        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }
        if (NetworkClient.localPlayer == null)
        {
            NetworkClient.AddPlayer();
        }
        while (NetworkClient.localPlayer == null)
        {
            Debug.Log("Waiting for local player...");
            yield return null;
        }


        GameObject.Find("MatchMaker").GetComponent<MatchMaker>().CreateLobby();
    }


    public void hostGame()
    {
        startButtonClicked();
        // this.GetComponent<Player>().HostGame(false);
    }
}
