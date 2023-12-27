using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class carMovement2 : NetworkBehaviour
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
    public LayerMask groundLayer;

    public Animator animator;
    int val;
    ///public Transform centerOfMass;

    private Rigidbody rb;

    void Awake()
    {
        //if (!IsOwner) return;

        //animator = GetComponent<Animator>();
        val = Animator.StringToHash("horizontal");
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, 0.1f, 0);
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
    }
    void FixedUpdate()
    {
        if (!IsOwner) return;

        //centerOfMass.position = rb.transform.position + rb.centerOfMass;
        //rb.centerOfMass = centerOfMass.position;
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        if(IsServer && IsLocalPlayer) move(moveInput, turnInput);
        else if(IsClient && IsLocalPlayer) move(moveInput, turnInput);



    }
    
    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(rayCastStartPosition.position, -transform.up, rayCastDistance);
        return hit;
    }

    private void move(float moveInput , float turnInput)
    {
        animator.SetFloat(val, (turnInput + 1) / 2);

        if (!IsGrounded())
        {
            rb.AddForce(-transform.up * extraGravity);
            //Debug.DrawRay(this.transform.position, -Vector3.up * extraGravity, Color.red);
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

        // Rotate the car based on user input for turning
        float temp = rb.velocity.magnitude;
        if (temp > 8) { temp = 8; }
        transform.Rotate(Vector3.up * turnInput * turnSpeed * temp);

        Vector3 rightVector = Vector3.Cross(Vector3.up, transform.forward);

        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross(Vector3.Cross(rb.velocity.normalized, transform.forward), transform.forward);
        // Debug.DrawRay(this.transform.position, lateralFrictionForce, Color.yellow);
        rb.AddForce(lateralFrictionForce);
        //Debug.DrawRay(rayCastStartPosition.position, -transform.up * rayCastDistance, Color.yellow);

        // Apply suspension force
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, suspensionHeight + 0.1f))
        {
            float suspensionCompression = 1f - (hit.distance / suspensionHeight);
            Vector3 suspensionForce = transform.up * suspensionCompression * suspensionForceMag;
            rb.AddForceAtPosition(suspensionForce, transform.position);
        }
    }

    [ServerRpc]
    private void MoveServerRPC(float moveInput, float turnInput)
    {
        move(moveInput, turnInput);
    }

}
