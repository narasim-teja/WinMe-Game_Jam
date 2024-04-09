using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oilPuddle : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tyre"))
        {
            StartCoroutine(oilCoroutine(other));
            
        }
    }

    IEnumerator oilCoroutine(Collider other)
    {
        
        float temp = other.GetComponentInParent<CarMovement2>().lateralFriction;
        other.GetComponentInParent<CarMovement2>().lateralFriction = 0f;
        other.GetComponentInParent<CarMovement2>().isOnOil = true;

        yield return new WaitForSeconds(5f);

        other.GetComponentInParent<CarMovement2>().lateralFriction = temp;
        other.GetComponentInParent<CarMovement2>().isOnOil = false;

    }

}
