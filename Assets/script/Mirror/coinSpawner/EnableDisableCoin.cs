using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public void Start(){
        DontDestroyOnLoad(this);
    }

    public void CoinPicked(GameObject coin)
    {
        // Debug.Log(coin.name);
        // Start the coroutine to disable and enable the coin
        StartCoroutine(EnableDisableRoutine(coin)); // This needs to be implemented as per your game logic
    }

    private IEnumerator EnableDisableRoutine(GameObject coin)
    {
        // Disable the coin
        coin.SetActive(false);

        // Wait for a random time between minWaitTime and maxWaitTime
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // Enable the coin
        // coin.SetActive(true);
        UpdateCoinStatus(coin);
        RpcUpdateCoinStatus(coin);
    }

    // Method to update the coin status on the server
    private void UpdateCoinStatus(GameObject coin)
    {
        if (coin == null)
        {
            Debug.LogError("Coin object is null in UpdateCoinStatus");
            return;
        }
        coin.SetActive(true);
        coin.GetComponent<CoinManager>().isPicked = false;
    }

    [ClientRpc] // This will be called on clients
    private void RpcUpdateCoinStatus(GameObject coin)
    {
        if (coin == null)
        {
            Debug.LogError("Coin object is null in RpcUpdateCoinStatus");
            return;
        }
        coin.SetActive(true);
        coin.GetComponent<CoinManager>().isPicked = false;
    }
}
