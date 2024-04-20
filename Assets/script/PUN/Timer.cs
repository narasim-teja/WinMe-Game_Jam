using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviourPun
{
    /*private Text timerText;
    private float startTime;
    private bool timerRunning = false;
    public static float elapsedTime;
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps.Add("tmr", Time.time);
        }
        timerText = this.GetComponent<Text>();
        StartTimer();
    }


    void Update()
    {
        if (timerRunning)
        {
            elapsedTime = Time.time - startTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        // Set the start time to the current time
        startTime = Time.time;
        timerRunning = true;
    }

    public void StopTimer()
    {   
        // Stop the timer
        timerRunning = false;
    }

    void UpdateTimerDisplay()
    {
        Debug.lo
        float elapsedTime = Time.time -(float)PhotonNetwork.CurrentRoom.CustomProperties["tmr"];
        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }*/

    // Example RPC to synchronize timer
    /* [PunRPC]
     void RPC_SyncTimer(float elapsedTime)
     {
         UpdateTimerDisplay(elapsedTime);
         *//*int minutes = Mathf.FloorToInt(elapsedTime / 60F);
         int seconds = Mathf.FloorToInt(elapsedTime % 60F);
         timerText = GetComponent<Text>();
         timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);*//*
     }*/
    public int matchLength = 180;
    public Text timerUI;

    private int currentMatchTime;
    private Coroutine timerCoroutine;

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps.Add("tmr", 0);
        }
        timerUI = this.GetComponent<Text>();
        InitializeTimer();

    }
    private void RefreshTimerUI()
    {
        float temp = (float)PhotonNetwork.CurrentRoom.CustomProperties["tmr"];
        int minutes = Mathf.FloorToInt(temp / 60F);
        int seconds = Mathf.FloorToInt(temp % 60F);
        timerUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void InitializeTimer()
    {
        currentMatchTime = matchLength;
        RefreshTimerUI();

        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(Timerr());
        }

    }
    private IEnumerator Timerr()
    {
        yield return new WaitForSeconds(1f);
        currentMatchTime -= 1;
        PhotonNetwork.CurrentRoom.CustomProperties["tmr"] = currentMatchTime;
        RefreshTimerUI();
        if (currentMatchTime <= 0)
        {
            timerCoroutine = null;

        }
        else
        {
            timerCoroutine = StartCoroutine(Timerr());
        }
    }
}
