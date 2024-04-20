using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviourPun
{
    public Text CoinCountText;
    private int CoinCount;

    public void OnTriggerEnter(Collider other)
    {
        
        if (!photonView.IsMine)
        {
            return;
        }
        Debug.Log(photonView.InstantiationId + "  coins :  " + CoinCount);
        if ((other.gameObject.CompareTag("coin") && other.GetComponent<CoinManager>().pickedUp == false))
        {
            other.GetComponent<CoinManager>().pickedUp = true;
            Debug.Log(photonView.InstantiationId + "  coins :  " + CoinCount);
            CoinCount++;
            CoinCountText.text = CoinCount.ToString();
            if (PhotonNetwork.IsMasterClient) Destroy(other.gameObject);
            
            
            
        }
    }
}
