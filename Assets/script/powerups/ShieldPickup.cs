using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject shield_active_prefab;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform powerupSlot = other.transform.Find("powerup_loc1");
            Quaternion adjustedRotation = other.transform.rotation * Quaternion.Euler(35, 0, 0);
            if (powerupSlot != null)
            {
                GameObject shieldInstance = Instantiate(
                    shield_active_prefab, 
                    powerupSlot.position, 
                    adjustedRotation,
                    powerupSlot 
                );

            }
            Destroy(gameObject);
        }
    }
}
