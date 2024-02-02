using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class PlayerScoreManager : NetworkBehaviour
{
    private int playerScore = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject canvas;
    GameObject temp;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            canvas.SetActive(false);
            return;
        }
    }
    /*private void Update()
    {
        if (!IsOwner)
        {
            canvas.SetActive(false);
            return;
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        
        //if (!IsOwner) return;
        if (other.CompareTag("coin"))
        {
            this.GetComponent<PlayerData>().Score.OnValueChanged = OnValueChange;

            if(IsServer) this.GetComponent<PlayerData>().Score.Value += 1;
            else ChangeScoreServerRPC();
 
            OnValueChange(0, this.GetComponent<PlayerData>().Score.Value);
           
            if (IsServer) other.gameObject.GetComponent<NetworkObject>().Despawn();
            else
            {
                temp = other.gameObject;
                //ulong networkId = other.GetComponent<NetworkObject>().NetworkObjectId;
                DestroyObjServerRPC();
            }

        }
    }

    private void OnValueChange(int previousValue, int newValue)
    {
        scoreText.text = newValue.ToString();
    }

    [ServerRpc(RequireOwnership =false)]
    private void DestroyObjServerRPC()
    {
        temp.GetComponent<NetworkObject>().Despawn();   
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeScoreServerRPC()
    {
        this.GetComponent<PlayerData>().Score.Value += 1;
    }



    /* public  void addPoints()
{

    playerScore += 1;
    UpdateScoreUI();
}


private void UpdateScoreUI()
{
    scoreText.text = playerScore.ToString();
}*/
}
