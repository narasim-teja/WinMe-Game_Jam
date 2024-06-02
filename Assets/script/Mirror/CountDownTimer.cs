using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CountDownTimer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTimerUpdate))]
    public float timeRemaining = 600f; // 10 minutes in seconds

    public Text timerText;

    public override void OnStartServer()
    {
        Debug.Log("check1");
        InvokeRepeating(nameof(UpdateTimer), 1.0f, 1.0f); // Start the timer on the server
        Debug.Log("check2");
    }

    [ServerCallback]
    void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= 1f;
            Debug.Log(timeRemaining);
            //UpdateTimerText(timeRemaining);
        }
        else
        {
            CancelInvoke(nameof(UpdateTimer));
            // Handle timer reaching zero (e.g., end the game)
        }
    }


    void OnTimerUpdate(float oldTime, float newTime)
    {
        UpdateTimerText(newTime);
    }

    void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string text = string.Format("{0:0}:{1:00}", minutes, seconds);
        timerText.text = text;
    }

    void Update()
    {
        if (!isServer)
        {
            UpdateTimerText(timeRemaining); // Update the UI for clients
        }
    }
}
