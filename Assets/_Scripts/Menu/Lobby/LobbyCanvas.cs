using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour {

    [SerializeField]
    private RoomLayoutGroup m_roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup {
        get { return m_roomLayoutGroup; }
    }

    [SerializeField]
    private GameObject m_errorScreen;
    private GameObject errorScreen {
        get { return m_errorScreen; }
    }

    public void OnClick_JoinRoom(string roomName) {
        if(PhotonNetwork.JoinRoom(roomName)) {
            //gg nao faz nada memo
        } else {
            print("Join room failed.");
            ToggleErrorScreen(true);
        }  
    }

    public void OnClick_Ok() {
        ToggleErrorScreen(false);
    }

    private void ToggleErrorScreen(bool active) {
        m_errorScreen.SetActive(active);
    }
}
