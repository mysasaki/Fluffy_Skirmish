using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveCurrentMatch : MonoBehaviour {

	public void OnClick_LeaveMatch() {
        print("leave match");

        DDOL ddol = PlayerManagement.Instance.GetComponentInParent<DDOL>();
        PhotonNetwork.Destroy(PlayerManagement.Instance.gameObject);
        PhotonNetwork.Destroy(PlayerNetwork.Instance.gameObject);
        Destroy(ddol.gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}
