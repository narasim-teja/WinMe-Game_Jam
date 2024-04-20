using Photon.Pun;
using UnityEngine;

public class TimerSingleton : MonoBehaviour
{
    /*private static TimerSingleton tmrInstance;

    public static TimerSingleton Instance
    {
        get
        {
            if (tmrInstance == null)
            {
                tmrInstance = FindObjectOfType<TimerSingleton>();

                if (tmrInstance == null)
                {
                    GameObject singletonObject = new GameObject("TimerSingleton");
                    tmrInstance = singletonObject.AddComponent<TimerSingleton>();

                    // Instantiate the singleton only on the master client
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.InstantiateRoomObject("TimerSingletonPrefab", Vector3.zero, Quaternion.identity);
                        // Set room property to indicate that the singleton has been instantiated
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "TimerSingletonExists", true } });
                    }
                }
            }
            return tmrInstance;
        }
    }*/

    void Awake()
    {
        // Ensure only one tmrInstance exists
        
    }

    /*private float startTime;
    public bool timerRunning = false;
    public float elapsedTime = 0;

    void Update()
    {
        if (tmrInstance != null && tmrInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            tmrInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (timerRunning)
        {
            elapsedTime = Time.time - startTime;
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }*/
}
