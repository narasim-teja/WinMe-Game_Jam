using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Mirror;
using Org.BouncyCastle.Asn1.Mozilla;
using UnityEngine.UIElements;

public enum Powerups : byte
{
    nothing,
    rocket,
    shield,
    burger
}

public class CarPowerupManager : NetworkBehaviour
{
    int burgerCount = 0;
    [SyncVar]
    public bool isShieldActive = false;
    public GameObject powerupHolder;
    public GameObject rocketPrefab;
    public GameObject rocketPrefabNetwokred;
    public GameObject shieldPrefab;
    public GameObject burgerPrefab;
    public ParticleSystem explosionEffect;
    public ParticleSystem shieldPowerupParticleEffect;
    [SyncVar(hook = nameof(OnChangePowerup))]
    public Powerups equippedPickup;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PowerUpManager();
        }
    }
    
    void PowerUpManager() {
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        if (powerup_loc1 != null) {
            if (powerup_loc1.childCount > 0) {
                GameObject powerup = powerup_loc1.GetChild(0).gameObject;

                if(powerup.gameObject.CompareTag("rocket")){
                    CmdDeleteRocketFromServer();
                    CmdRequestFireRocket();
                }
                if(powerup.gameObject.CompareTag("burger")){
                    if(burgerCount < 2) {
                        burgerCount++;
                        StartCoroutine(CmdConsumeBurger(powerup));
                    }
                }
                if(powerup.gameObject.CompareTag("shield") ){
                    if(isShieldActive == false) StartCoroutine(ConsumeShield(powerup));
                    else Debug.Log("!!! Shield already in use !!!");
                }
            } else {
                Debug.Log("!!! No Powerup picked up yet !!!");
            }
        } else {
            Debug.LogError("!!! powerup holder object not found !!!");
        }
    }
    [Server]
    public void SetEquippedPickupSyncVar(NetworkConnection target, Powerups powerupName)
    {
        equippedPickup = powerupName;
        if (isServer)
        {
            StartCoroutine(ChangeEquipment(equippedPickup));
        }
    }
    void OnChangePowerup(Powerups oldEquippedItem, Powerups newEquippedItem)
    {
        StartCoroutine(ChangeEquipment(newEquippedItem));
    }
    // Since Destroy is delayed to the end of the current frame, we use a coroutine
    // to clear out any child objects before instantiating the new one
    IEnumerator ChangeEquipment(Powerups newEquippedItem)
    {
        while (powerupHolder.transform.childCount > 0)
        {
            Destroy(powerupHolder.transform.GetChild(0).gameObject);
            yield return null;
        }

        switch (newEquippedItem)
        {
            
            case Powerups.rocket:
                InstantiateRocket(powerupHolder.transform);
                break;
            case Powerups.shield:
                Instantiate(shieldPrefab, powerupHolder.transform);
                break;
            case Powerups.burger:
                Instantiate(burgerPrefab, powerupHolder.transform);
                break;
        }
        
    }

//🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀

    void InstantiateRocket(Transform powerupHolder){
        Quaternion adjustedRotation = this.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

        GameObject rocketInstance = Instantiate(
            rocketPrefab, 
            powerupHolder.position, 
            adjustedRotation,
            powerupHolder 
        );
        Instantiate(rocketInstance);
    }


    [Command]
    void CmdDeleteRocketFromServer(){
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        GameObject powerup = powerup_loc1.GetChild(0).gameObject;
        Destroy(powerup);
        ClientRpcDeleteRocketFromAllClients();
        equippedPickup = Powerups.nothing;
    }
    [ClientRpc]
    void ClientRpcDeleteRocketFromAllClients(){
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        GameObject powerup = powerup_loc1.GetChild(0).gameObject;
        Destroy(powerup);
    }

    [Command]
    void CmdRequestFireRocket()
    {   
        Quaternion adjustedRotation = this.transform.rotation * Quaternion.Euler(90f, 0f, 0f);
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");

        GameObject rocketInstance = Instantiate(
            rocketPrefabNetwokred, 
            powerup_loc1.position, 
            adjustedRotation 
        );
        NetworkServer.Spawn(rocketInstance);

        RocketFired rocketScript = rocketInstance.GetComponent<RocketFired>();
        if (rocketScript != null)
        {
            NetworkIdentity clientNetId = this.GetComponent<NetworkIdentity>(); 
            rocketScript.SetParentID(clientNetId.netId);
        }
        rocketInstance.GetComponent<BoxCollider>().enabled = true;

        // Server validates and executes the rocket launch
        if (rocketInstance != null) {
            Rigidbody rocketRb = rocketInstance.GetComponent<Rigidbody>();
            if (rocketRb != null) {
                rocketRb.isKinematic = false;
                float rocketLaunchForce = 1500f;
                rocketRb.AddForce(rocketInstance.transform.up * rocketLaunchForce);
            }
        }
    }

    [TargetRpc]
    public void TargetRPCApplyExplosionForceOnClient(NetworkConnection target, Vector3 forceDirection , Vector3 upwardDirection, Vector3 randomTorque ,float explosionForce ){
        this.GetComponent<Rigidbody>().AddForce((forceDirection + upwardDirection) * explosionForce, ForceMode.Impulse);
        this.GetComponent<Rigidbody>().AddTorque(randomTorque, ForceMode.Impulse);
        
    }

    [ClientRpc]
    public void ClientRpcPlayExplosionEffect(Transform explosionPosition){
        ParticleSystem instance = Instantiate(explosionEffect, explosionPosition.position, Quaternion.identity);
        instance.Play();
        Destroy(instance.gameObject, instance.main.duration);
    }
//🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀


//🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔
    IEnumerator CmdConsumeBurger(GameObject child_powerup){
        float scalingFactor = 1.5f;
        float scaleX = scalingFactor * transform.localScale.x;
        float scaleY = scalingFactor * transform.localScale.y;
        float scaleZ = scalingFactor * transform.localScale.z;
        wheelLogic[] wheelLogicScripts = this.gameObject.GetComponentsInChildren<wheelLogic>();

        foreach(wheelLogic wheelLogicScript in wheelLogicScripts )
            wheelLogicScript.suspensionHeight = wheelLogicScript.suspensionHeight * scalingFactor ;

        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        Destroy(child_powerup);

        yield return new WaitForSeconds(10);
        wheelLogicScripts = this.gameObject.GetComponentsInChildren<wheelLogic>();
        foreach(wheelLogic wheelLogicScript in wheelLogicScripts )
            wheelLogicScript.suspensionHeight = wheelLogicScript.suspensionHeight/ scalingFactor ;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
        transform.localScale = new Vector3(scaleX / scalingFactor, scaleY / scalingFactor, scaleZ / scalingFactor);
        burgerCount--;
    }
//🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔🍔


//🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡
    IEnumerator ConsumeShield(GameObject child_powerup){
        ParticleSystem instance = Instantiate(shieldPowerupParticleEffect, transform.position, Quaternion.identity);
        instance.transform.SetParent(transform);
        isShieldActive = true;
        Destroy(child_powerup);

        yield return new WaitForSeconds(10);

        isShieldActive = false;
        Destroy(instance.gameObject);
    }
//🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡🛡

}