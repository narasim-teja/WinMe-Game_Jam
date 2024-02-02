using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class coinCollector : NetworkBehaviour
{

    public AudioSource collectSound;
    public GameObject canvas;
    public override void OnNetworkSpawn()
    {
        //if (!IsOwner) return;
    }
    void OnTriggerEnter(Collider other)
    {
        //if (!IsOwner) return;
        
        //canvas.GetComponentInChildren<PlayerScoreManager>().addPoints();
        //canvas.GetComponent<PlayerScoreManager>().addPoints();
        /*if (!IsOwner) return;
        
        NetworkVariable<int> temp = scoringSystem.theScore;
        scoringSystem.theScore.Value = temp.Value + 1;
        
        

        collectSound.Play();
        Destroy(gameObject);
        */
    }

}
