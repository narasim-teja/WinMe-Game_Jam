using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
public class RocketPickup : NetworkBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject rocket_active_prefab;

    void Update()
    {
        // Rotate the power-up slowly over time
        RotatePowerup(); 
    }

    void RotatePowerup(){
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CmdSpawnActiveRocket(other.gameObject);
        }
    }
    [Server]
    void CmdSpawnActiveRocket(GameObject other){
            // Transform powerupSlot = other.transform.Find("powerup_loc1");

            // if (powerupSlot != null)
            // {
                // Quaternion adjustedRotation = other.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

                // GameObject rocketInstance = Instantiate(
                //     rocket_active_prefab, 
                //     powerupSlot.position, 
                //     adjustedRotation,
                //     powerupSlot 
                // );
                // NetworkServer.Spawn(rocketInstance);

                // RocketFired rocketScript = rocketInstance.GetComponent<RocketFired>();
                // if (rocketScript != null)
                // {
                //     rocketScript.SetParentID(other.GetInstanceID());
                // }

                //targetclientRPC to set the rocket active on the client
                NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    // other.GetComponent<CarPowerupManager>().TargetSetRocketParent(networkIdentity.connectionToClient, rocketInstance);
                    other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient , Powerups.rocket);
                }   
            // }
        NetworkServer.Destroy(this.gameObject);
    }
}
