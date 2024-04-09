using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _roomName;

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) {
            Debug.Log("not connected");
            return;
        }
        else
        {
            Debug.Log("is connected");
        }
        RoomOptions roomOps = new RoomOptions();
        roomOps.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, roomOps, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnCreateRoomFailed(short returncode, string message)
    {
        Debug.Log("Room Creation failed");
    }

}
