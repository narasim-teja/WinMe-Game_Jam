using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Mirror;

public class carMovementOffline : MonoBehaviour
{
    public float acceleration = 1000f;
    public float maxSpeed = 32f;
    public float turnSpeed = 0.3f;
    public float suspensionHeight = 0.55f;
    public float suspensionForceMag = 350f;
    public float lateralFriction = -3500f;
    public float extraGravity = 100f;
    public float rayCastDistance = 0.55f;
    public Transform[] rayCastStartPositions;
    Vector3 rotationSmoothVelocity;
    public LayerMask groundLayer;
    public bool isOnOil = false;
    public bool isOnSlime = false;


    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;
    // [SerializeField] AudioSource kartDriftAudioSource;

    //public Animator animator;
    int val;

    private Rigidbody rb;
    private float originalAcceleration;
    private float originalLaterationFriction;

    // public float raycastDistance = 1.8f;
    //powerUps
    int burgerCount = 0;
    public bool isShieldActive = false;
    public ParticleSystem shieldPowerupParticleEffect;


    void Awake()
    {
        originalAcceleration = acceleration;
        originalLaterationFriction = lateralFriction;
        val = Animator.StringToHash("horizontal");
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        Vector3 worldCenterOfMass = rb.transform.TransformPoint(rb.centerOfMass);
        //Debug.DrawRay(worldCenterOfMass, transform.up * raycastDistance, Color.red);

        // inputs
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        
        move(moveInput, turnInput);

        // PowerUpManager();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PowerUpManager();
        }
    }

    private bool IsGrounded()
    {
        bool hit = false;
        int groundLayerMask = LayerMask.GetMask("ground");

        foreach (Transform rayCastStartPosition in rayCastStartPositions)
        {
            hit = hit || Physics.Raycast(rayCastStartPosition.position, -transform.up, rayCastDistance, groundLayerMask);
        }
        return hit;
    }

    private void move(float moveInput, float turnInput)
    {
        //animator.SetFloat(val, (turnInput + 1) / 2);
        RotateCar(new Vector3(0, 1, 0), 1f);

        // preventing car from going haywire while in 


        //rb.AddForce(-Vector3.up * extraGravity);
        //Debug.Log("on ground");
        // Accelerate and decelerate
        if (!IsGrounded())
        {
            leftTrail.emitting = false;
            rightTrail.emitting = false;

            // if (kartDriftAudioSource.isPlaying)
            //     kartDriftAudioSource.Stop();
            return;
        }
        float currentSpeed = Vector3.Dot(rb.velocity, transform.forward);
        float desiredSpeed = moveInput * maxSpeed;
        float accelerationForce = (desiredSpeed - currentSpeed) * acceleration;


        //rb.AddForce(transform.forward * accelerationForce);
        if (moveInput < 0)
        {
            turnInput = -turnInput;
            accelerationForce /= 3;
            rb.AddForce(transform.forward * accelerationForce);

        }
        else if (moveInput >= 0)
        {
            rb.AddForce(transform.forward * accelerationForce);
        }
        /*else if(moveInput == 0) {
            
        }*/
        //rb.AddForceAtPosition(transform.forward * accelerationForce, this.transform.position - new Vector3(0, 0.5f, 0));
        // Limit the maximum speed
        // Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Rotate the car 
        float temp = rb.velocity.magnitude;
        if (temp > 8) { temp = 8; }
        transform.Rotate(Vector3.up * turnInput * turnSpeed * temp);
        Vector3 rightVector = Vector3.Cross(Vector3.up, transform.forward);


        //LateralFriction to prevent drifting

        if (isOnOil == true)
        {
            //acceleration = 5f;
            //leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }
        else if (isOnSlime == true)
        {
            //acceleration = 0.5f;
            //rb.velocity /= (rb.velocity.magnitude / 5f);

            leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.green;
            rightTrail.startColor = Color.green;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            lateralFriction = originalLaterationFriction / 3;
            acceleration = originalAcceleration / 2;
            leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;

            // if (!kartDriftAudioSource.isPlaying)
            // {
            //     kartDriftAudioSource.Play();
            // }
        }
        else
        {
            lateralFriction = originalLaterationFriction;
            acceleration = originalAcceleration;
            leftTrail.emitting = false;
            rightTrail.emitting = false;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;


            // if (kartDriftAudioSource.isPlaying)
            // {
            //     kartDriftAudioSource.Stop();
            // }
        }


        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross(Vector3.Cross(rb.velocity.normalized, transform.forward), transform.forward);
        rb.AddForce(lateralFrictionForce);
        // print(lateralFrictionForce);
        //Debug.DrawRay(rayCastStartPosition.position, -transform.up * rayCastDistance, Color.yellow);

    }


    void RotateCar(Vector3 surfaceNormal, float rotationSpeed)
    {
        // Calculate the target rotation based on the hit surface normal
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

        // Smoothly rotate the car towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void PowerUpManager() {
        Transform powerup_loc1 = this.transform.Find("powerup_loc1");
        if (powerup_loc1 != null) {
            if (powerup_loc1.childCount > 0) {
                GameObject powerup = powerup_loc1.GetChild(0).gameObject;

                if(powerup.gameObject.CompareTag("rocket")){
                    FireRocket(powerup);
                }
                if(powerup.gameObject.CompareTag("burger")){
                    if(burgerCount < 2) {
                        burgerCount++;
                        StartCoroutine(ConsumeBurger(powerup));
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
            Debug.LogError("!!! powerup1 object not found !!!");
        }
    }


    void FireRocket(GameObject child_powerup){
        child_powerup.transform.SetParent(null,true);
        Rigidbody rocketRb = child_powerup.GetComponent<Rigidbody>(); 
        if (rocketRb != null)
        {
            // rocketRb.GetComponent<RocketFired>().isFired = true;
            rocketRb.isKinematic = false;
            float launchForce = 1500f;
            rocketRb.AddForce(child_powerup.transform.up * launchForce);
        }
    }

    IEnumerator ConsumeBurger(GameObject child_powerup){
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

    IEnumerator ConsumeShield(GameObject child_powerup){
        ParticleSystem instance = Instantiate(shieldPowerupParticleEffect, transform.position, Quaternion.identity);
        instance.transform.SetParent(transform);
        isShieldActive = true;
        Destroy(child_powerup);

        yield return new WaitForSeconds(10);

        isShieldActive = false;
        Destroy(instance.gameObject);
    }
}
