using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using Org.BouncyCastle.Crypto.Paddings;

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
        Debug.Log("-----------");
        Tuple<uint, string, int> winner = calculateWinner();
        showWinner(winner.Item1, winner.Item2 ,winner.Item3);
    }

    [Server]
    public Tuple<uint, string ,int> calculateWinner()
    {
        uint winnerNetId = 0;
        string winnerName = "";
        int highestCoinCount = -1;

        for (int x = 0; x < players.Length; x++)
        {
            NetworkIdentity networkIdentity = players[x].GetComponent<NetworkIdentity>();

            String playerName = players[x].GetComponent<PlayerManager>().playerName;
            CarUIManager carUIManager = players[x].GetComponentInChildren<CarUIManager>();

            if (networkIdentity != null && carUIManager != null)
            {
                int coinCount = carUIManager.coinCount;
                Debug.Log(networkIdentity.netId + "----" + coinCount);

                if (coinCount > highestCoinCount)
                {
                    highestCoinCount = coinCount;
                    winnerName = playerName;
                    winnerNetId = networkIdentity.netId;
                }
            }
            else
            {
                Debug.LogError("Missing NetworkIdentity or CarUIManager component on player " + players[x].name);
            }
        }

        Debug.Log("Winner name: " + winnerName + " with coin count: " + highestCoinCount);
        return new Tuple<uint , string, int>(winnerNetId, winnerName ,highestCoinCount);
    }
    [ClientRpc]
    public void showWinner(uint winnerId, string winnerName ,int coinAmount)
    {
        winnerUI.transform.Find("winnerId").GetComponent<TextMeshProUGUI>().text = winnerName;
        winnerUI.transform.Find("winnerCoinCount").GetComponent<TextMeshProUGUI>().text = coinAmount.ToString();

        winnerUI.gameObject.SetActive(true);
    }
}
