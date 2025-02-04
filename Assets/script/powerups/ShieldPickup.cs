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
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            DisablePowerup(other.gameObject);
            CmdSpawnActiveShield(other.gameObject);
            Invoke(nameof(RespawnPowerup), 5f);
        }
    }

    [ServerCallback]
    void CmdSpawnActiveShield(GameObject other){
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient, Powerups.shield);
        }
    }

    [ClientRpc]
    void DisablePowerup(GameObject player)
    {
        player.transform.Find("Audio").GetComponent<CarAudio>().PlayPowerup();
        gameObject.SetActive(false);
    }

    [ServerCallback]
    void RespawnPowerup()
    {
        Spawner.SpawnRandomPowerup(transform.position);
        NetworkServer.Destroy(this.gameObject);
    }
}
