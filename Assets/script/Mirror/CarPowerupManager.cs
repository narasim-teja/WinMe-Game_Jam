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
    [SyncVar]
    public int burgerCount = 0;
    [SyncVar]
    public bool isShieldActive = false;
    public GameObject powerupHolder;
    public GameObject rocketPrefab;
    public GameObject rocketPrefabNetwokred;
    public GameObject shieldPrefab;
    public GameObject burgerPrefab;
    public ParticleSystem explosionEffect;
    public ParticleSystem shieldPowerupParticleEffect;
    public ParticleSystem shieldInstance;
    [SyncVar(hook = nameof(OnChangePowerup))]
    public Powerups equippedPickup;
    float burgerScalingFactor = 1.5f;

    [SerializeField] AudioSource burgerBiteAudioSource;
    [SerializeField] AudioSource burgerBuffDownAudioSource;
    [SerializeField] AudioSource shieldUpAudioSource;
    [SerializeField] AudioSource shieldDownAudioSource;

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
                    CmdDeleteTopPowerupFromServer();
                    CmdRequestFireRocket();
                }
                if(powerup.gameObject.CompareTag("burger")){
                    CmdDeleteTopPowerupFromServer();
                    if(burgerCount < 2) {
                        CmdIncreaseBurgerCount();
                        // burgerCount++;
                        CmdConsumeBurger();
                    }
                }
                if(powerup.gameObject.CompareTag("shield") ){
                    CmdDeleteTopPowerupFromServer();
                    if(isShieldActive == false) CmdConsumeShield();//StartCoroutine(ConsumeShield(powerup));
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
                Quaternion rocketAdjustedRotation = this.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

                Instantiate(rocketPrefab,powerupHolder.transform.position, rocketAdjustedRotation, powerupHolder.transform);
                if(isLocalPlayer) CmdSpawnRocketOnServer();
                
                break;
            case Powerups.shield:
                Quaternion shieldAdjustedRotation = this.transform.rotation * Quaternion.Euler(-60f, 0f, 0f);
                Instantiate(shieldPrefab,powerupHolder.transform.position, shieldAdjustedRotation , powerupHolder.transform);
                if(isLocalPlayer) CmdSpawnShieldOnServer();
                
                break;
            case Powerups.burger:
                Instantiate(burgerPrefab,powerupHolder.transform.position,shieldPrefab.transform.rotation , powerupHolder.transform);
                if(isLocalPlayer) CmdSpawnBurgerOnServer();
                
                break;
        }
        
    }



    [Command]
    void CmdDeleteTopPowerupFromServer(){
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        GameObject powerup = powerup_loc1.GetChild(0).gameObject;
        Destroy(powerup);
        ClientRpcDeleteTopPowerupFromAllClients();
        equippedPickup = Powerups.nothing;
    }
    [ClientRpc]
    void ClientRpcDeleteTopPowerupFromAllClients(){
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        GameObject powerup = powerup_loc1.GetChild(0).gameObject;
        Destroy(powerup);
    }

    #region Rocket Logic
    [Command]
    public void CmdSpawnRocketOnServer()
    {
        Instantiate(rocketPrefab, powerupHolder.transform);
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
    #endregion


    #region Burger Logic
    [Command]
    void CmdSpawnBurgerOnServer()
    {
        Instantiate(burgerPrefab, powerupHolder.transform);
    }
    
    
    [Command]
    public void CmdIncreaseBurgerCount(){
        burgerCount++;
    }

    [Command]
    public void CmdConsumeBurger()
    {
        // Call a coroutine on the server side
        StartCoroutine(ConsumeBurgerCoroutine());
    }
    [Server]
    IEnumerator ConsumeBurgerCoroutine(){
        RpcScaleUp();

        yield return new WaitForSeconds(10);

        RpcScaleDown();

        burgerCount--;
    }
    [ClientRpc]
    void RpcScaleUp()
    {
        ScaleCar(burgerScalingFactor);
        burgerBiteAudioSource.Play();
    }

    [ClientRpc]
    void RpcScaleDown()
    {
        burgerBuffDownAudioSource.Play();
        ScaleCar(1 / burgerScalingFactor);

    }

    void ScaleCar(float factor)
    {
        // Adjust the scale of the car
        float scaleX = factor * transform.localScale.x;
        float scaleY = factor * transform.localScale.y;
        float scaleZ = factor * transform.localScale.z;
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        // Adjust the suspension height
        wheelLogic[] wheelLogicScripts = gameObject.GetComponentsInChildren<wheelLogic>();
        foreach (wheelLogic wheelLogicScript in wheelLogicScripts)
        {
            wheelLogicScript.suspensionHeight *= factor;
        }
    }
    #endregion


    #region Shield Logic
    [Command]
    void CmdSpawnShieldOnServer()
    {
        Instantiate(shieldPrefab, powerupHolder.transform);
    }
    
    [Command]
    public void CmdConsumeShield()
    {
        // Call a coroutine on the server side
        StartCoroutine(ConsumeShieldCoroutine());
    }
    [Server]
    IEnumerator ConsumeShieldCoroutine(){
        RpcConsumeShield();
        isShieldActive = true;

        yield return new WaitForSeconds(15);

        // Deactivate the shield
        RpcDeactivateShield();
        isShieldActive = false;
    }
    [ClientRpc]
    public void RpcConsumeShield()
    {
        if (shieldInstance == null)
        {
            shieldUpAudioSource.Play();
            shieldInstance = Instantiate(shieldPowerupParticleEffect, transform.position, Quaternion.identity);
            shieldInstance.transform.SetParent(transform);
        }
    }

    [ClientRpc]
    public void RpcDeactivateShield()
    {
        if (shieldInstance != null)
        {
            shieldDownAudioSource.Play();
            Destroy(shieldInstance.gameObject);
            shieldInstance = null;
        }
    }
    #endregion

}