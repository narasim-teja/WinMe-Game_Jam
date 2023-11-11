using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carMovement2 : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 20f;
    public float turnSpeed = 0.05f;
    public float suspensionHeight = 1f;
    public float suspensionForceMag = 500f;
    public float lateralFriction = -300f;
    public float extraGravity = 2000f;
    public float rayCastDistance = 1f;
    public LayerMask groundLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
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
        
        if (moveInput < 0) { 
            accelerationForce /= 3;
            rb.AddForce(transform.forward * accelerationForce * 100);

        }
        if (moveInput >0) rb.AddForce(transform.forward * accelerationForce * 100);

        // Limit the maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Rotate the car based on user input for turning
        float temp = rb.velocity.magnitude;
        if(temp > 8) { temp = 8; }
        transform.Rotate(Vector3.up * turnInput * turnSpeed * temp);

        Vector3 rightVector = Vector3.Cross(Vector3.up, transform.forward);

        Vector3 lateralFrictionForce = -rb.velocity.magnitude * lateralFriction * Vector3.Cross( Vector3.Cross(rb.velocity.normalized , transform.forward) , transform.forward);
        Debug.DrawRay(this.transform.position, lateralFrictionForce,Color.yellow );
        rb.AddForce(lateralFrictionForce);

        // Apply suspension force
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, suspensionHeight + 0.1f))
        {
            Debug.Log("afaga");
            float suspensionCompression = 1f - (hit.distance / suspensionHeight);
            Vector3 suspensionForce = transform.up * suspensionCompression * suspensionForceMag;
            rb.AddForceAtPosition(suspensionForce, transform.position);
        }
        

    }
    
    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(transform.position, -transform.up, rayCastDistance);
        return hit;
    }

}
