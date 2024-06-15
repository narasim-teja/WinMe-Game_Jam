using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnableDisableCoin : Mirror.NetworkBehaviour
{
    public GameObject coinPrefab;
    public float minWaitTime = 3f;
    public float maxWaitTime = 10f;

    private List<GameObject> coins = new List<GameObject>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Find all coins in the scene and add them to the list
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("coin"))
        {
            coins.Add(coin);
        }
    }

    public void CoinPicked(GameObject coin)
    {
        Debug.Log(coin.name);
        // Start the coroutine to disable and enable the coin
        StartCoroutine(EnableDisableRoutine(coin)); // This needs to be implemented as per your game logic
    }

    private IEnumerator EnableDisableRoutine(GameObject coin)
    {
        // Disable the coin
        coin.SetActive(false);

        // Wait for a random time between minWaitTime and maxWaitTime
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        float remainingTime = waitTime;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            remainingTime -= 1f;
        }

        // Enable the coin
        coin.SetActive(true);
        
        RpcUpdateCoinStatus(coin);
    }

    [Mirror.ClientRpc]
    private void RpcUpdateCoinStatus(GameObject coin) {
        coin.SetActive(true);
        coin.GetComponent<CoinManager>().isPicked = false;
    }
}
