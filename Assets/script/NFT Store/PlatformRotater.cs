using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePlatform : MonoBehaviour
{ 
    [SerializeField]
    private float rotationSpeed = 50f;

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
