using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public bool isTrail = false;

    public float radius = 0.5f;       // Radius of the spiral
    public float cycleDuration = 8f; // Time for one full up-down cycle (seconds)
    public float distance = 2f;
    public Vector3 treasureBoxPosition;

    // Other object variables
    private float rotationSpeed = 50f;

    void Update()
    {
        if (isTrail)
        {
            float verticalSpeed = distance / cycleDuration;
            float y = Mathf.PingPong(Time.time * verticalSpeed, 1f);

            // Calculate angle based on time (continuous spiral)
            float angularSpeed = (8 * Mathf.PI) / cycleDuration; // 8π radians per cycle
            float theta = angularSpeed * Time.time;

            // Convert polar coordinates (theta, radius) to Cartesian (x, z)
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            // Update position
            //transform.position = new Vector3(x, y, z);
            transform.localPosition = treasureBoxPosition + new Vector3(x, y, z);
        }
        else
        {
            this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
