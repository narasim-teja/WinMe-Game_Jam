using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using Org.BouncyCastle.Crypto.Paddings;
using UnityEngine.SocialPlatforms;
using Thirdweb;
using System.Threading.Tasks;
using Unity.Collections;

public class UserData{

    public string user_name;

    public int coinCount;

    public string walletAddress=null;

}
public class GameManager : NetworkBehaviour
{
    GameObject[] players;

    [SerializeField]
    GameObject pickUpLocationParent;
    public GameObject mainMenuUI;

    List<UserData> userDataList = new List<UserData>();

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
        Debug.Log("-----GAME ENDED------");
        GetUserData();
        userDataList.Sort((user1, user2) => user2.coinCount.CompareTo(user1.coinCount));
        UserData winner = userDataList[0];
        showWinner(winner.user_name ,winner.coinCount);

        UpdateCoinsEarned();
    }
    
    [Server]
    public void GetUserData(){
        for (int x = 0; x < players.Length; x++)
        {
            NetworkIdentity networkIdentity = players[x].GetComponent<NetworkIdentity>();
            CarUIManager carUIManager = players[x].GetComponentInChildren<CarUIManager>();
            NetworkConnection conn = networkIdentity.connectionToClient;
            string playerName = players[x].GetComponent<PlayerManager>().playerInfo.name;
            int coin_count = carUIManager.coinCount;

            string walletAddress = MirrorNetworkManager.singleton.connToWalletMap[conn.connectionId];
            CmdSetWalletAddressForClient(walletAddress,playerName,coin_count);
        }
    }

    [Server]
    public void CmdSetWalletAddressForClient(string walletAddress,string playerName, int coin_count){
        UserData currentUser = new UserData();

        currentUser.walletAddress = walletAddress;
        currentUser.user_name = playerName;
        currentUser.coinCount = coin_count;

        userDataList.Add(currentUser);
    }
    [Server]
    public void UpdateCoinsEarned()
    {
        for(int x = 0 ; x < userDataList.Count ; x++)
        {
            UserData currentUser = userDataList[x];

            int coins_earned = 0;
            if(x == 0) coins_earned = userDataList[x].coinCount;
            else if(x > 0 && x < 5) coins_earned = userDataList[x].coinCount / (2*x);
            
            if(currentUser.walletAddress != null )  SupaBaseClient.addMoneyToDb(currentUser.walletAddress,coins_earned,currentUser.user_name); 

        }
    }
    [ClientRpc]
    public void showWinner(string winnerName ,int coinAmount)
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
