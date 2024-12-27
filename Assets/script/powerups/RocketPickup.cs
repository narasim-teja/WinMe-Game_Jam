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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            DisablePowerup(other.gameObject);
            CmdSpawnActiveRocket(other.gameObject);
            Invoke(nameof(RespawnPowerup), 5f);
        }
    }
    [ServerCallback]
    void CmdSpawnActiveRocket(GameObject other){
        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            other.GetComponent<CarPowerupManager>().SetEquippedPickupSyncVar(networkIdentity.connectionToClient, Powerups.rocket);
        }
        gameObject.SetActive(false);
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
