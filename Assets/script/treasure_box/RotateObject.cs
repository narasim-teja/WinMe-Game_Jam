using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public bool isTrail = false;

    public float speed = 2f;  // Speed of movement
    public float radius = 2f; // Radius of spiral
    public float heightLimit = 2f; // Max Y position
    public int rings = 2; // Number of spiral rings per up/down cycle

    private float angle = 0f;
    private int direction = 1; // 1 for up, -1 for down
    private float heightStep;


    // Other object variables
    private float rotationSpeed = 50f;
    void Start()
    {
        heightStep = heightLimit / rings; // Calculate height change per ring
    }

    void Update()
    {
        if (isTrail)
        {
            // Spiral motion
            angle += speed * Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float y = direction * (angle / (2 * Mathf.PI)) * heightStep;

            // Reverse direction when reaching the top or bottom
            if (Mathf.Abs(y) >= heightLimit)
            {
                direction *= -1; // Flip direction (up/down)
                angle = 0f; // Reset angle to start a new spiral cycle
            }

            transform.position = new Vector3(x, y, z);
        }
        else
        {
            this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
