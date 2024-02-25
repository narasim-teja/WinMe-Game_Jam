using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class carMovement2 : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 35f;
    public float turnSpeed = 0.3f;
    public float suspensionHeight = 1f;
    public float suspensionForceMag = 300f;
    public float lateralFriction = -300f;
    public float extraGravity = 1000f;
    public float rayCastDistance = 0.55f;
    public Transform rayCastStartPosition;
    Vector3 rotationSmoothVelocity;
    public LayerMask groundLayer;
    public bool isOnOil = false;
    public bool isOnSlime = false;

    private Quaternion originalRotation;
    public float rotationSpeed = 10f;

    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;

    public Animator animator;
    int val;

    private Rigidbody rb;


    private float offGroundTime = 0f;
    public float raycastDistance = 5.0f;
    void Awake()
    {
        val = Animator.StringToHash("horizontal");
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, 0.1f, 0);

        originalRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        // inputs
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        move(moveInput, turnInput);
    }
    
    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(rayCastStartPosition.position, -transform.up, rayCastDistance);
        return hit;
    }

    private void move(float moveInput , float turnInput)
    {
        animator.SetFloat(val, (turnInput + 1) / 2);
    
        // preventing car from going haywire while in air 
        if (!IsGrounded())
        {
            offGroundTime += Time.deltaTime;

            if (offGroundTime > 0.5f)
            {
                RaycastHit hit1;
                if (Physics.Raycast(transform.position, Vector3.down, out hit1, raycastDistance))
                {
                    Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.green);
                    RotateCar(hit1.normal);
                }
                else
                {
                    Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
                }
            }


            rb.AddForce(-Vector3.up * extraGravity);
            return;
        }
        else
        {
            offGroundTime = 0f;
            originalRotation = transform.rotation;
        }

        // Accelerate and decelerate
        float currentSpeed = Vector3.Dot(rb.velocity, transform.forward);
        float desiredSpeed = moveInput * maxSpeed;
        float accelerationForce = (desiredSpeed - currentSpeed) * acceleration;

        if (moveInput < 0)
        {
            turnInput = -turnInput;
            accelerationForce /= 3;
            rb.AddForce(transform.forward * accelerationForce * 100);

        }
        if (moveInput > 0)
        {
            rb.AddForce(transform.forward * accelerationForce * 100);
        }

        // Limit the maximum speed
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
        if(isOnOil == true)
        {
            acceleration = 5f;
            leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }
        else if(isOnSlime == true)
        {
            acceleration = 0.5f;
            rb.velocity /=(rb.velocity.magnitude /5f) ;

            leftTrail.emitting = true;
            rightTrail.emitting = true;
            
            leftTrail.startColor = Color.green;
            rightTrail.startColor = Color.green;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            lateralFriction = -50;
            acceleration = 1f;
            leftTrail.emitting = true;
            rightTrail.emitting = true;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }
        else
        {
            lateralFriction = -300;
            acceleration = 5f;
            leftTrail.emitting = false;
            rightTrail.emitting = false ;

            leftTrail.startColor = Color.black;
            rightTrail.startColor = Color.black;
        }

        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross(Vector3.Cross(rb.velocity.normalized, transform.forward), transform.forward);
        rb.AddForce(lateralFrictionForce);
        Debug.DrawRay(rayCastStartPosition.position, -transform.up * rayCastDistance, Color.yellow);


        // Apply suspension force
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, suspensionHeight + 0.1f))
        {
            float suspensionCompression = 1f - (hit.distance / suspensionHeight);
            Vector3 suspensionForce = transform.up * suspensionCompression * suspensionForceMag;
            rb.AddForceAtPosition(suspensionForce, transform.position);
        }*/
    }


    void RotateCar(Vector3 surfaceNormal)
    {
        // Calculate the target rotation based on the hit surface normal
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

        // Smoothly rotate the car towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


}
