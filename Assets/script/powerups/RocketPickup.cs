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
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient , Powerups.rocket);
        }   
        NetworkServer.Destroy(this.gameObject);
    }
}
