using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class CountDownTimer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTimerUpdate))]
    private int timeRemaining = 3;
    private readonly int gameDuration = 10;
    private bool isCountDown = true;

    public Text timerText;
    [SerializeField]
    private GameObject dialogueMessage;
    GameManager gameManager;

    public override void OnStartServer()
    {
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
        if (timeRemaining == 0 && isCountDown)
        {
            isCountDown = false;
            timeRemaining = gameDuration;
            BeginKartMove();
        }
        else if (timeRemaining == 0)
        {
            gameManager.GameEnded();
        }
        else if(timeRemaining == -5)
        {
            ShowReturningMessage();
        }
        else if (timeRemaining == -15)
        {
            CancelInvoke(nameof(UpdateTimer));
            StopClients();
            StopServer();
        }
    }

    [ClientRpc]
    void ShowReturningMessage()
    {
        dialogueMessage.SetActive(true);
    }

    [ClientRpc]
    void BeginKartMove()
    {
        if (NetworkClient.localPlayer.TryGetComponent<carMovement3>(out carMovement3 moveScript))
        {
            moveScript.SetKartPausedState(false);
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

    public void ReturnToMainMenu()
    {
        MirrorNetworkManager.singleton.StopClient();
    }

    #region Timer Functions
    void OnTimerUpdate(int oldTime, int newTime) 
    {
        UpdateTimerText(newTime);
    }

    void UpdateTimerText(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        if (timeRemaining >= 0 && !isServer)
        {
            string text = string.Format("{0:0}:{1:00}", minutes, seconds);
            timerText.text = text;
        } 
        else if(timeRemaining <= -5)
        {
            TextMeshProUGUI messageText = dialogueMessage.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = $"Returning to main menu in {seconds + 15}s";
            }
        }
    }
    #endregion
}
