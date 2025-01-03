using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime,Space.World);
    }
}
