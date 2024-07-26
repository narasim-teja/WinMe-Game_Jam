using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelLogic : MonoBehaviour
{
    // Start is called before the first frame update
    Transform rayCastStartPosition;
    public float suspensionHeight = 0.55f;
    public float suspensionForceMag = 4000f;
    Rigidbody rb;
    public float dampingCoefficient = 250f;

    public GameObject wheelPrefab;
    void Start()
    {
        rb = this.GetComponentInParent<Rigidbody>();
        rayCastStartPosition = this.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wheelSuspension();
    }

    void wheelSuspension()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayCastStartPosition.position, -transform.up, out hit, suspensionHeight))
        {
            float originalsuspensionCompression = 1f - (hit.distance / suspensionHeight);
            float suspensionCompression = Mathf.Clamp01(originalsuspensionCompression);
            //Debug.Log(suspensionCompression);

            // Suspension force
            Vector3 suspensionForce = transform.up * suspensionCompression * suspensionForceMag;

            // Calculate the damping force
            Vector3 velocity = rb.GetPointVelocity(rayCastStartPosition.position);
            Vector3 dampingForce = -dampingCoefficient * Vector3.Project(velocity, transform.up);

            // Apply suspension and damping forces
            rb.AddForceAtPosition(suspensionForce + dampingForce, rayCastStartPosition.position);
            //Debug.DrawRay(rayCastStartPosition.position, (suspensionForce + dampingForce), Color.red);
            
            Debug.DrawRay(rayCastStartPosition.position, -transform.up * hit.distance, Color.green);


            wheelPrefab.gameObject.transform.position = hit.point + new Vector3(0,0.12f,0);
                
            //Debug.Log(hit.point);
        }
    }
}
