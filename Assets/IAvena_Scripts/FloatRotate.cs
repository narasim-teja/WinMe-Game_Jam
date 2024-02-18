using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// put on floating collectables to make them float and rotate

public class FloatRotate : MonoBehaviour
{
    public float degreesPerSecond = 15f;
    public float amplitude = 0.25f; // how high the item floats
    public float frequency = 1f;

    // position storage variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    void Start()
    {
        //store starting orientation
        posOffset = transform.position;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // float with Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}
