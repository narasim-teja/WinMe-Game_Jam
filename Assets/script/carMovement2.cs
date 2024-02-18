using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class carMovement2 : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 33f;
    public float turnSpeed = 0.04f;
    public float suspensionHeight = 1f;
    public float suspensionForceMag = 300f;
    public float lateralFriction = -300f;
    public float extraGravity = 1000f;
    public float rayCastDistance = 0.55f;
    public Transform rayCastStartPosition;
    Vector3 rotationSmoothVelocity;
    public LayerMask groundLayer;

    private Quaternion originalRotation;
    public float rotationSpeed = 2f;

    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;

    public Animator animator;
    int val;

    private Rigidbody rb;

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

        //rotationRestrictions();
    
        // preventing car from going haywire while in air 
        if (!IsGrounded())
        {
            reverseRotation();
            rb.AddForce(-Vector3.up * extraGravity);
            return;
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
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            lateralFriction = -50;
            acceleration = 1f;
            leftTrail.emitting = true;
            rightTrail.emitting = true;

        }
        else
        {
            lateralFriction = -300;
            acceleration = 5f;
            leftTrail.emitting = false;
            rightTrail.emitting = false ;
        }

        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross(Vector3.Cross(rb.velocity.normalized, transform.forward), transform.forward);
        rb.AddForce(lateralFrictionForce);
        Debug.DrawRay(rayCastStartPosition.position, -transform.up * rayCastDistance, Color.yellow);


        // Apply suspension force
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, suspensionHeight + 0.1f))
        {
            float suspensionCompression = 1f - (hit.distance / suspensionHeight);
            Vector3 suspensionForce = transform.up * suspensionCompression * suspensionForceMag;
            rb.AddForceAtPosition(suspensionForce, transform.position);
        }
    }

    // preventing car from flipping mid air by always trying to bring it to neutral/original rotation
    private void reverseRotation()
    {
        float originalYRotation = originalRotation.eulerAngles.y;

        Quaternion targetRotation = Quaternion.Euler(
            originalRotation.eulerAngles.x,
            Mathf.MoveTowardsAngle(transform.eulerAngles.y, originalYRotation, Time.deltaTime * rotationSpeed),
            originalRotation.eulerAngles.z
        );

        transform.rotation = targetRotation;
    }

    // restricting rotation so that car do not flip in mid air
    // currently not implemented because causing some issue where car was hoping like crazy
    private void rotationRestrictions()
    {
        float currentRotationx = transform.eulerAngles.x;
        float currentRotationz = transform.eulerAngles.z;

        float clampedRotationx = 0f;
        if (currentRotationx > 150f)  clampedRotationx = Mathf.Clamp(currentRotationx, 340f,360f );
        else clampedRotationx = Mathf.Clamp(currentRotationx, 0f, 20f);

        float clampedRotationz = 0f;
        if (currentRotationz > 150f) clampedRotationz = Mathf.Clamp(currentRotationz, 340f, 360f);
        else clampedRotationz = Mathf.Clamp(currentRotationz, 0f, 20f);

        Vector3 currentRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        Vector3 newRotation = new Vector3(clampedRotationx, transform.eulerAngles.y, clampedRotationz);
        transform.eulerAngles = Vector3.SmoothDamp(currentRotation, newRotation, ref rotationSmoothVelocity, 0.02f);
    }

  

}
