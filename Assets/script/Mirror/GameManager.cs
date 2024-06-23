using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class GameManager : NetworkBehaviour
{
    GameObject[] players;

    
    public GameObject winnerUI;
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    [Server]
    public void GameEnded()
    {
        Tuple<uint, int> winner = calculateWinner();
        Debug.Log("!!!!!!!");
        showWinner(winner.Item1,winner.Item2);
    }

    [Server]
    public Tuple<uint,int> calculateWinner()
    {
        uint winnerNetId = 0;
        int highestCoinCount = -1;

        for (int x = 0; x < players.Length; x++)
        {
            NetworkIdentity networkIdentity = players[x].GetComponent<NetworkIdentity>();
            CarUIManager carUIManager = players[x].GetComponentInChildren<CarUIManager>();

            if (networkIdentity != null && carUIManager != null)
            {
                int coinCount = carUIManager.coinCount;
                //Debug.Log(networkIdentity.netId + "----" + coinCount);

                if (coinCount > highestCoinCount)
                {
                    highestCoinCount = coinCount;
                    winnerNetId = networkIdentity.netId;
                }
            }
            else
            {
                Debug.LogError("Missing NetworkIdentity or CarUIManager component on player " + players[x].name);
            }
        }

        //Debug.Log("Winner NetID: " + winnerNetId + " with coin count: " + highestCoinCount);
        return new Tuple<uint, int>(winnerNetId, highestCoinCount);
    }
    [ClientRpc]
    public void showWinner(uint winnerId, int coinAmount)
    {
        winnerUI.transform.Find("winnerId").GetComponent<TextMeshProUGUI>().text = winnerId.ToString();
        winnerUI.transform.Find("winnerCoinCount").GetComponent<TextMeshProUGUI>().text = coinAmount.ToString();

        winnerUI.gameObject.SetActive(true);
    }
}
