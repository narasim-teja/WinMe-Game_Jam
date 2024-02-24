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
        
        float temp = other.GetComponentInParent<carMovement2>().lateralFriction;
        other.GetComponentInParent<carMovement2>().lateralFriction = 0f;
        other.GetComponentInParent<carMovement2>().isOnOil = true;

        yield return new WaitForSeconds(5f);

        other.GetComponentInParent<carMovement2>().lateralFriction = temp;
        other.GetComponentInParent<carMovement2>().isOnOil = false;

    }

}
