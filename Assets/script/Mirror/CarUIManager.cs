// using Mirror;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;

// public class CarUIManager : NetworkBehaviour
// {
//     [SerializeField] private Text coinText;

//     [SyncVar]
//     public int coinCount;

//     private EnableDisableCoin enableDisableCoin;

//     public override void OnStartServer()
//     {
//         base.OnStartServer();
//         setEnableDisableCoin();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (!isLocalPlayer) { return; }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!isLocalPlayer) return;

//         if(enableDisableCoin == null) setEnableDisableCoin();

//         if (other.gameObject.CompareTag("coin"))
//         {
//             if (!other.gameObject.GetComponent<CoinManager>().isPicked)
//             {
//                 //this should be in Command but putting it in command leads to 
//                 //picking up of coin multiple time  FIX THIS LATER
//                 other.GetComponent<CoinManager>().isPicked = true;

//                 CmdHandleCoinPickup(other.gameObject);
//             }
//         }
//     }

//     [Command]
//     private void setEnableDisableCoin()
//     {
//         enableDisableCoin = FindObjectOfType<EnableDisableCoin>();
//     }

//     [Command]
//     private void CmdHandleCoinPickup(GameObject coin)
//     {
//         if (coin != null && !coin.GetComponent<CoinManager>().isPicked)
//         {
//             coin.GetComponent<CoinManager>().isPicked = true;
//             coinCount++;

//             // Update the coin status and notify clients
//             enableDisableCoin.CoinPicked(coin);
//             RpcHandleCoinPickup(coin);
//             RpcUpdateCoinText(coinCount);
//         }
//     }

//     [ClientRpc]
//     private void RpcHandleCoinPickup(GameObject coin)
//     {
//         if (coin != null)
//         {
//             coin.SetActive(false);
//         }
//     }

//     [ClientRpc]
//     private void RpcUpdateCoinText(int newCoinCount)
//     {
//         if (coinText != null)
//         {
//             coinText.text = newCoinCount.ToString();
//         }
//     }
// }

using System.Collections;
using Mirror;
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
        Debug.Log("!!!!!!!!!!!");
        // StartCoroutine(WaitForConnectionAndSetEnableDisableCoin());
        if(NetworkServer.active == true) Debug.Log("connected");
        if(NetworkServer.active == false) Debug.Log("NOTconnected");
        setEnableDisableCoin();
    }
    // [Client]
    // private IEnumerator WaitForConnectionAndSetEnableDisableCoin()
    // {
    //     Debug.Log("-------------");
    //     while (!NetworkServer.)
    //     {
    //         Debug.Log("connecting to network...");
    //         yield return null;
    //     }
    //     if(NetworkServer.active == true) Debug.Log("connected");
    //     if(NetworkServer.active == false) Debug.Log("NOTconnected");
    //     setEnableDisableCoin();
    // }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.gameObject.CompareTag("coin"))
        {
            CoinManager coinManager = other.gameObject.GetComponent<CoinManager>();
            if (coinManager != null && !coinManager.isPicked)
            {
                // Local check for coin pickup, but actual logic will be handled by the server
                CmdHandleCoinPickup(other.gameObject);
            }
        }
    }

    [Server]
    private void setEnableDisableCoin()
    {
        enableDisableCoin = FindObjectOfType<EnableDisableCoin>();
    }

    [Command]
    private void CmdHandleCoinPickup(GameObject coin)
    {
        if (coin == null) return;

        CoinManager coinManager = coin.GetComponent<CoinManager>();
        if (coinManager != null && !coinManager.isPicked)
        {
            coinManager.isPicked = true;
            coinCount++;

            // Update the coin status and notify clients
            enableDisableCoin.CoinPicked(coin);
            RpcHandleCoinPickup(coin);
            RpcUpdateCoinText(coinCount);
        }
    }

    [ClientRpc]
    private void RpcHandleCoinPickup(GameObject coin)
    {
        if (coin != null)
        {
            coin.SetActive(false);
        }
    }

    [ClientRpc]
    private void RpcUpdateCoinText(int newCoinCount)
    {
        if (coinText != null)
        {
            coinText.text = newCoinCount.ToString();
        }
    }
}
