using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectToMenu : MonoBehaviour {

    public GameObject ddol;

	public void OnClick_BackMenu() {
        PhotonNetwork.Destroy(PlayerManagement.Instance.gameObject);
        PhotonNetwork.Destroy(PlayerNetwork.Instance.gameObject);
        Destroy(ddol);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }
}
