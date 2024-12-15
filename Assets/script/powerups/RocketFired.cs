using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class RocketFired : NetworkBehaviour
{
    private uint parentNetworkID;
    // public bool isFired = false;
    private float upwardForce = 3f;
    private float explosionForce = 5000f; 
    private float torqueForce = 1000f;
    public bool rocketCollided = false;
    public ParticleSystem explosionEffect;
    public void SetParentID(uint id)
    {
        parentNetworkID = id;
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if(rocketCollided == true) return;
        if(!other.CompareTag("ground") && !other.CompareTag("Player")) return;

        if(other.CompareTag("ground")){
            ClientRpcPlayExplosionEffect();
            rocketCollided = true;
            NetworkServer.Destroy(gameObject);
            return;
        }
        if(other.GetComponent<NetworkIdentity>() && other.GetComponent<NetworkIdentity>().netId == parentNetworkID) return;

        rocketCollided = true;

        Vector3 forceDirection = (other.transform.position - transform.position).normalized;
        Vector3 upwardDirection = Vector3.up * upwardForce;
        Vector3 randomTorque = new Vector3( 0, Random.Range(-1f, 1f) , 0) * torqueForce;

        if (other.GetComponent<NetworkIdentity>() && other.GetComponent<NetworkIdentity>().netId != parentNetworkID && other.CompareTag("Player"))
        {
            if(other.GetComponent<CarPowerupManager>().isShieldActive == false){
                Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();

                if (targetRigidbody != null)
                {
                    // Call target RPC to the client because simulating explosion on server is not possible since the client network transform is synced from client to server
                    NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
                    other.GetComponent<CarPowerupManager>().TargetRPCApplyExplosionForceOnClient(networkIdentity.connectionToClient, forceDirection, upwardDirection, randomTorque, explosionForce);      
                }
            }
            other.GetComponent<CarPowerupManager>().ClientRpcPlayExplosionEffect(other.GetComponent<Transform>());
            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void ClientRpcPlayExplosionEffect(){
        ParticleSystem instance = Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        instance.Play();
        Destroy(instance.gameObject, instance.main.duration);
    }
}
