using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Mirror;
public class BurgerPickup : NetworkBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject burger_active_prefab;

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
            CmdSpawnActiveBurger(other.gameObject);
        }
    }
    [Server]
    void CmdSpawnActiveBurger(GameObject other){
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient , Powerups.burger);
        }   
        NetworkServer.Destroy(this.gameObject);
    }
}
