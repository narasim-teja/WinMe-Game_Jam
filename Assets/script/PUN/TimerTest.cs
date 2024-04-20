using UnityEngine;
using Photon.Pun;
using Com.MyCompany.MyGame;

public class TimerTest : MonoBehaviourPunCallbacks
{
    private static GameObject timerObjectInstance;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timerObjectInstance == null && GameObject.Find("timerUI") == null)
            {
                Debug.Log("hiiiiiiiiiii" + GameManager.Instance.getSpawn());
                if (GameManager.Instance.getSpawn() == 0)
                {

                    GameManager.Instance.inrSpawn();
                    timerObjectInstance = PhotonNetwork.InstantiateRoomObject("timerUI", Vector3.zero, Quaternion.identity);
                    Debug.Log("hiiiiiiiiiii" + GameManager.Instance.getSpawn());
                }
            }
        }
    }

    /* [PunRPC]
     private void InitializeTimer(float duration)
     {
         timer = duration;
     }

     void Update()
     {
         if (timer > 0)
         {
             timer -= Time.deltaTime;
             // Update timer UI here
             Debug.Log("Timer: " + timer);
         }
         else
         {
             // Timer has reached zero, do something (e.g., end the game)
         }
     }*/
}
