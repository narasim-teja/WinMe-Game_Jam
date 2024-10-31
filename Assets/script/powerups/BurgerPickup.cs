using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BurgerPickup : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject burger_active_prefab;

    void Update()
    {
        // Rotate the power-up slowly over time
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform powerupSlot = other.transform.Find("powerup_loc1");

            if (powerupSlot != null)
            {
                GameObject burgerInstance = Instantiate(
                    burger_active_prefab, 
                    powerupSlot.position, 
                    quaternion.identity,
                    powerupSlot 
                );

            }

            Destroy(gameObject);
        }
    }
}
