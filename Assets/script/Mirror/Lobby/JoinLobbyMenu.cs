using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager manager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    // private void OnEnable()
    // {
    //     networkManager.OnClientConnected += HandleClientConnected;
    //     networkManager.OnClientDisconnected += HandleClientDisconnected;
    // }

    // private void OnDisable()
    // {
    //     NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
    //     NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    // }

    public void JoinLobby()
    {
        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        string ipAddress = ipAddressInputField.text;

        // manager.networkAddress = ipAddress;
        manager.StartClient();
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
                NetworkClient.AddPlayer();
            Debug.Log(manager.networkAddress);

            Player.localPlayer.JoinGame(ipAddress);
        }


        // joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}