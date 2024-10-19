using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RocketPickup : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject rocket_active_prefab;

    void Update()
    {
        // Rotate the power-up slowly over time
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform powerupSlot = other.transform.Find("powerup1");

            if (powerupSlot != null)
            {
                Quaternion adjustedRotation = other.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

                GameObject rocketInstance = Instantiate(
                    rocket_active_prefab, 
                    powerupSlot.position, 
                    adjustedRotation,
                    powerupSlot 
                );

                //setting parent id So that the rocket does not hit the parent player
                RocketFired rocketScript = rocketInstance.GetComponent<RocketFired>();
                if (rocketScript != null)
                {
                    rocketScript.SetParentID(other.GetInstanceID());
                }
            }

            Destroy(gameObject);
        }
    }
}
