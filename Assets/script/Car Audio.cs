using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CarAudio : NetworkBehaviour
{
    [SerializeField] AudioSource engineSound;
    public float minPitch = 0.4f;
    public float maxPitch = 1.5f;
    float carPitch;

    Rigidbody rb;
    void Start()
    {
        rb = FindAnyObjectByType<Rigidbody>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        engineSound.Play();
    }

    void Update()
    {
        carPitch = rb.velocity.magnitude / 15f;
        // Debug.Log(carPitch);
        if (carPitch < minPitch)
        {
            engineSound.pitch = minPitch;
        }
        else if (carPitch > maxPitch)
        {
            engineSound.pitch = maxPitch;
        }
        else
        {
            engineSound.pitch = carPitch;
        }
    }
}
