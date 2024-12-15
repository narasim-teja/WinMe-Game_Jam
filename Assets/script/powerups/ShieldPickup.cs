using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Mirror;
public class ShieldPickup : NetworkBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject shield_active_prefab;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CmdSpawnActiveShield(other.gameObject);
        }
    }
    [Server]
    void CmdSpawnActiveShield(GameObject other){
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient , Powerups.shield);
        }   
        NetworkServer.Destroy(this.gameObject);
    }
}
