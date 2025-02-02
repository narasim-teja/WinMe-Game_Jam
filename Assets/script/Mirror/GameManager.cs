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

    [SerializeField]
    GameObject pickUpLocationParent;

    
    public GameObject winnerUI;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Canvas canvas2;
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        players = GameObject.FindGameObjectsWithTag("Player");
        InitPowerups();
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

            string playerName = players[x].GetComponent<PlayerManager>().playerInfo.name;
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
        winnerText.text = winnerName;
        amountText.text = coinAmount.ToString();

        winnerUI.SetActive(true);
        EnableWinEffect();
    }

    private void EnableWinEffect()
    {
        GameObject kartCamera = GameObject.Find(Constants.KART_CAMERA);
        if (kartCamera != null)
        {
            Transform winEffectTransform = kartCamera.transform.Find(Constants.WIN_EFFECT_OBJ);
            if (winEffectTransform != null)
            {
                GameObject WinObj = winEffectTransform.gameObject;
                Camera winObjCam = WinObj.GetComponent<Camera>();

                canvas2.renderMode = RenderMode.ScreenSpaceCamera;
                canvas2.worldCamera = winObjCam;
                WinObj.SetActive(true);
            }
        }
    }

    [Server]
    void InitPowerups()
    {
        foreach (Transform child in pickUpLocationParent.transform)
        {
            Spawner.SpawnRandomPowerup(child.position);
        }
    }
}
