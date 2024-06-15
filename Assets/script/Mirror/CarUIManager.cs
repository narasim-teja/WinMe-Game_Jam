using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarUIManager : NetworkBehaviour
{
    [SerializeField] private Text coinText;

    [SyncVar]
    public int coinCount;

    private EnableDisableCoin enableDisableCoin;

    public override void OnStartServer()
    {
        base.OnStartServer();
        setEnableDisableCoin();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.gameObject.CompareTag("coin"))
        {
            if (!other.gameObject.GetComponent<CoinManager>().isPicked)
            {
                //this should be in Command but putting it in command leads to 
                //picking up of coin multiple time  FIX THIS LATER
                other.GetComponent<CoinManager>().isPicked = true;
                
                CmdHandleCoinPickup(other.gameObject);
            }
        }
    }

    [Command]
    private void setEnableDisableCoin()
    {
        enableDisableCoin = FindObjectOfType<EnableDisableCoin>();
    }

    [Command]
    private void CmdHandleCoinPickup(GameObject coin)
    {
        
        enableDisableCoin = FindObjectOfType<EnableDisableCoin>();
        RpcHandleCoinPickup(coin);
        //NetworkServer.Destroy(coin);
        coinCount++;
        enableDisableCoin.CoinPicked(coin);
        RpcUpdateCoinText(coinCount);
    }

    [ClientRpc]
    private void RpcHandleCoinPickup(GameObject coin)
    {
        if (coin != null)
        {
            // Destroy the coin on all clients
            //Destroy(coin);
            coin.SetActive(false);
        }
    }

    [ClientRpc]
    private void RpcUpdateCoinText(int newCoinCount)
    {
        coinText.text = newCoinCount.ToString();
    }
}
