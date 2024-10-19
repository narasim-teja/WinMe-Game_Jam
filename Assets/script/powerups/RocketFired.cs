// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class RocketFired : MonoBehaviour
// {
//     private int parentID;

//     public void SetParentID(int id)
//     {
//         parentID = id;
//     }
//     private void OnTriggerEnter(Collider other) {
//         if(other.GetInstanceID() != parentID){
//             Destroy(this.gameObject);
//         }
//         Debug.Log("" + other.gameObject.name);
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFired : MonoBehaviour
{
    private int parentID;
    public float upwardForce = 500f;  // Adjust force value based on your game scale
    public float explosionForce = 500f;  // Adjust the overall force of the explosion
    public float torqueForce = 500f;  // Adjust rotational force

    public void SetParentID(int id)
    {
        parentID = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit is not the parent
        if (other.GetInstanceID() != parentID)
        {
            // Try to get the Rigidbody component of the object hit
            Rigidbody targetRigidbody = other.attachedRigidbody;

            if (targetRigidbody != null)
            {
                Vector3 forceDirection = (other.transform.position - transform.position).normalized;
                Vector3 upwardDirection = Vector3.up * upwardForce;

                targetRigidbody.AddForce((forceDirection + upwardDirection/10) * explosionForce, ForceMode.Impulse);

                // Apply random torque (rotation) to give the object a spinning effect
                Vector3 randomTorque = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ) * torqueForce;

                targetRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
            }

            Destroy(this.gameObject);
        }

        // Debugging info for which object is hit
        Debug.Log("Hit object: " + other.gameObject.name);
    }
}
