using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class scoringSystem : NetworkBehaviour
{
    public GameObject scoreText;
    //public static int theScore;
    //public static NetworkVariable<int> theScore = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    void Update()
    {
        //if (!IsOwner) return;
        //scoreText.GetComponent<Text>().text = theScore.Value.ToString();
    }
}
