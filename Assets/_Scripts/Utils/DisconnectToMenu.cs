using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectToMenu : MonoBehaviour {

	public void OnClick_BackMenu() {
        Destroy(PlayerManagement.Instance.gameObject);
        Destroy(PlayerNetwork.Instance.gameObject);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
