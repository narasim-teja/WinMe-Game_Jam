using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUIManager : NetworkBehaviour
{
    [SerializeField] private Text coinText;
    private int coinCount;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            if (!other.gameObject.GetComponent<CoinManager>().isPicked)
            {
                other.gameObject.GetComponent<CoinManager>().isPicked = true;
                NetworkServer.Destroy(other.gameObject);
                UpdateCoinText();
            }
        }
    }

    private void UpdateCoinText()
    {
        coinCount++;
        coinText.text = coinCount.ToString();
    }

}
