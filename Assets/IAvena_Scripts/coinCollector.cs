using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinCollector : MonoBehaviour
{

    public AudioSource collectSound;

    void OnTriggerEnter(Collider other)
    {
        //print("collected!");
        //theScore += 1;

        // scoringSystem.theScore += 1;
        // Destroy(gameObject);
        collectSound.Play();
    }

}
