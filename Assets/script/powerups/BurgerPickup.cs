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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            DisablePowerup();
            CmdSpawnActiveBurger(other.gameObject);
            Invoke(nameof(RespawnPowerup), 5f);
        }
    }
    [ServerCallback]
    void CmdSpawnActiveBurger(GameObject other){
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient , Powerups.burger);
        }
    }

    [ClientRpc]
    void DisablePowerup()
    {
        gameObject.SetActive(false);
    }

    [ServerCallback]
    void RespawnPowerup()
    {
        Spawner.SpawnRandomPowerup(transform.position);
        NetworkServer.Destroy(this.gameObject);
    }
}
