using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateMass : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.mass = 700f;
    }

}
