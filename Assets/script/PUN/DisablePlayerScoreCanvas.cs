using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayerScoreCanvas : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            this.gameObject.SetActive(false);
        }
    }

}
