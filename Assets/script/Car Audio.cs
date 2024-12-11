using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] AudioSource engineSound;
    public float minPitch = 0.4f;
    public float maxPitch = 1.5f;
    float carPitch;

    Rigidbody rb;
    void Start()
    {
        // engineSound = GetComponent<AudioSource>();
        // rb = GetComponent<Rigidbody>();
        rb = FindAnyObjectByType<Rigidbody>();
        engineSound.pitch = minPitch;
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
