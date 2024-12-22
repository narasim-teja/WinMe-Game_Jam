using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CarUIManager : NetworkBehaviour
{
    [SerializeField] private Text coinText;

    [SyncVar (hook = nameof(OnCoinCountChanged))]
    public int coinCount;

    [SerializeField] AudioSource coinPickUpAudioSource;

    void OnCoinCountChanged(int oldCoinCount, int newCoinCount)
    {
        if (coinText != null)
        {
            coinText.text = newCoinCount.ToString();
        }
    }
}
