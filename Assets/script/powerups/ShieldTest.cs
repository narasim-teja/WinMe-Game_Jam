using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTest : MonoBehaviour
{
    public GameObject rocket;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireRocket());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator FireRocket(){
        GameObject rocketInstance = Instantiate(rocket,this.gameObject.transform);
        
        Rigidbody rocketRb = rocketInstance.GetComponent<Rigidbody>(); 
        if (rocketRb != null)
        {
            rocketRb.isKinematic = false;
            float launchForce = 1500f;
            rocketRb.AddForce(rocketInstance.transform.up * launchForce);
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(FireRocket());
    }
}
