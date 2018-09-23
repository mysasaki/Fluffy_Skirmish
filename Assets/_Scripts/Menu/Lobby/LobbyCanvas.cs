using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour {

    [SerializeField]
    private RoomLayoutGroup m_roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup {
        get { return m_roomLayoutGroup; }
    }

    public void OnClick_JoinRoom(string roomName) {
        if(PhotonNetwork.JoinRoom(roomName)) {
            //gg nao faz nada memo
        } else {
            print("Join room failed.");
        }  
    }
}
