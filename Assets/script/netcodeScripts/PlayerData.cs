using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<int> Score = new NetworkVariable<int>(0 ,NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Server);
    //public NetworkVariable<string> Name = new NetworkVariable<string>();
}
