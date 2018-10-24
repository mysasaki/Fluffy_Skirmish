﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    private Text m_roomName;
    private Text roomName {
        get { return m_roomName; }
    }

    public void OnClick_CreateRoom() {

        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 9
        };
        
        if (PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default)) {
            print("Create room successfully sent!");

        } else {
            print("Create room failed to send");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage) {
        print("Create room failed: " + codeAndMessage[1]);
    }
    

    private void OnCreatedRoom() {
        print("Room created successfully");
    }
}
