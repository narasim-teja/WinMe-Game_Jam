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
    private float upwardForce = 3f;
    private float explosionForce = 5000f; 
    private float torqueForce = 1000f;

    public ParticleSystem explosionEffect;
    public void SetParentID(int id)
    {
        parentID = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetInstanceID() != parentID && other.tag == "Player" )
        {
            Rigidbody targetRigidbody = other.attachedRigidbody;

            if (targetRigidbody != null)
            {
                Vector3 forceDirection = (other.transform.position - transform.position).normalized;
                Vector3 upwardDirection = Vector3.up * upwardForce;

                targetRigidbody.AddForce((forceDirection + upwardDirection) * explosionForce, ForceMode.Impulse);

                Vector3 randomTorque = new Vector3( 0, Random.Range(-1f, 1f) , 0) * torqueForce;

                targetRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

                ParticleSystem instance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                instance.Play();

                Destroy(instance.gameObject, instance.main.duration);
            }
            Destroy(this.gameObject);
        }
    }
}
