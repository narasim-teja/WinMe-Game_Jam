using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Mirror;

public class carMovement3 : NetworkBehaviour
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

    //public Animator animator;
    int val;

    private Rigidbody rb;
    private float originalAcceleration;
    private float originalLaterationFriction;

    // public float raycastDistance = 1.8f;

    void Awake()
    {
        originalAcceleration = acceleration;
        originalLaterationFriction = lateralFriction;
        val = Animator.StringToHash("horizontal");
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        Vector3 worldCenterOfMass = rb.transform.TransformPoint(rb.centerOfMass);
        //Debug.DrawRay(worldCenterOfMass, transform.up * raycastDistance, Color.red);

        // inputs
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        move(moveInput, turnInput);
    }

    private bool IsGrounded()
    {
        bool hit = false;
        int groundLayerMask = LayerMask.GetMask("ground");

        foreach (Transform rayCastStartPosition in rayCastStartPositions) {
            hit = hit || Physics.Raycast(rayCastStartPosition.position, -transform.up, rayCastDistance,groundLayerMask);
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
        if (!IsGrounded()) {
            leftTrail.emitting = false;
            rightTrail.emitting = false;
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
            rb.AddForce(transform.forward * accelerationForce );

        }
        else if (moveInput >= 0)
        {
            rb.AddForce(transform.forward * accelerationForce );
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
        else if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            lateralFriction = originalLaterationFriction/ 3;
            acceleration = originalAcceleration /2;
            leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }
        else
        {
            lateralFriction = originalLaterationFriction;
            acceleration = originalAcceleration;
            leftTrail.emitting = false;
            rightTrail.emitting = false;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }


        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross(Vector3.Cross(rb.velocity.normalized, transform.forward), transform.forward);
        rb.AddForce(lateralFrictionForce);
        //Debug.DrawRay(rayCastStartPosition.position, -transform.up * rayCastDistance, Color.yellow);
        
    }


    void RotateCar(Vector3 surfaceNormal, float rotationSpeed)
    {
        // Calculate the target rotation based on the hit surface normal
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

        // Smoothly rotate the car towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
