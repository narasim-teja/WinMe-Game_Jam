using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimePuddle : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tyre"))
        {
            StartCoroutine(slimeCoroutine(other));

        }
    }

    IEnumerator slimeCoroutine(Collider other)
    {
        other.GetComponentInParent<carMovement2>().isOnSlime = true;

        yield return new WaitForSeconds(2f);

        other.GetComponentInParent<carMovement2>().isOnSlime = false;

    }
}
