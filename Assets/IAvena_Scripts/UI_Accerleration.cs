using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Accerleration : MonoBehaviour
{

    // when holding the spacebar (Input.GetKeyDown) increase _Removed Segments

    public Material speedMaterial;
    public bool accelerating;

    void Start()
    {
        speedMaterial.SetFloat("_RemovedSegments", 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Okay, you hit it");
            //speedMaterial.SetFloat("_RemovedSegments", 5);
            accelerating = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            print("oops! you let go"); // decrease _Removed Segments
            speedMaterial.SetFloat("_RemovedSegments", 0);
            accelerating = false;
        }

        if (accelerating)
        {
            //Gradually increase the value of "_RemovedSegments" to 5
            float newValue = Mathf.Lerp(speedMaterial.GetFloat("_RemovedSegments"), 5f, Time.deltaTime);
            speedMaterial.SetFloat("_RemovedSegments", newValue);
        }
        else
        {
            //Gradually decrease the value of "_RemovedSegments" to 0
            float newValue = Mathf.Lerp(speedMaterial.GetFloat("_RemovedSegments"), 0f, Time.deltaTime);
            speedMaterial.SetFloat("_RemovedSegments", newValue);
        }



    }

}
