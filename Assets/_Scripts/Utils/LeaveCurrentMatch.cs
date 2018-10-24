﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveCurrentMatch : MonoBehaviour {

	public void OnClick_LeaveMatch() {
        print("leave match");
        Destroy(PlayerManagement.Instance.gameObject);
        Destroy(PlayerNetwork.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
}
