using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CountDownTimer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTimerUpdate))]
    private int timeRemaining = 60; 

    public Text timerText;
    GameManager gameManager;
    MirrorNetworkManager networkManager;

    public override void OnStartServer()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<MirrorNetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager == null)
        {
            Debug.Log("GameManager EMPTY");
        }
        InvokeRepeating(nameof(UpdateTimer), 1.0f, 1.0f); // Start the timer on the server
    }

    [ServerCallback]
    void UpdateTimer()
    {
        timeRemaining -= 1;
        if (timeRemaining == 0)
        {
            gameManager.GameEnded();
        }
        else if (timeRemaining == -7)
        {
            CancelInvoke(nameof(UpdateTimer));
            StopClients();
            StopServer();
        }
    }

    [ServerCallback]
    async void StopServer()
    {
        await DeployApi.Instance.StopServerGracefully();
    }

    [ClientRpc]
    void StopClients()
    {
        MirrorNetworkManager.singleton.StopClient();
    }

    void OnTimerUpdate(int oldTime, int newTime) 
    {
        UpdateTimerText(newTime);
    }

    void UpdateTimerText(int time) 
    {
        if (timeRemaining >= 0)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            string text = string.Format("{0:0}:{1:00}", minutes, seconds);
            timerText.text = text;
        }
    }

    void Update()
    {
        if (!isServer)
        {
            UpdateTimerText(timeRemaining); // Update the UI for clients
        }
    }
}
