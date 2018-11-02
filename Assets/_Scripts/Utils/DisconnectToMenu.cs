using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectToMenu : MonoBehaviour {

    private GameObject ddol;

	public void OnClick_BackMenu() {
        ddol = GameObject.FindObjectOfType<DDOL>().gameObject;
        PhotonNetwork.Destroy(PlayerManagement.Instance.gameObject);
        PhotonNetwork.Destroy(PlayerNetwork.Instance.gameObject);
        Destroy(ddol);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }
}
