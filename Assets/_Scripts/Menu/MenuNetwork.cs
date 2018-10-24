using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNetwork : MonoBehaviour {

    [SerializeField]
    private GameObject m_loadingScreen;
    private GameObject loadingScreen {
        get { return m_loadingScreen; }
    }

    [SerializeField]
    private GameObject m_errorScreen;
    private GameObject errorScreen {
        get { return m_errorScreen; }
    }

    public void OnClick_Play() {
        if (!PhotonNetwork.connected) {
            print("Connecting to server");
            PhotonNetwork.ConnectUsingSettings("0.1"); //conecta ao servidor

            ToggleLoadingScreen(true);
        }
    }

    public void OnClick_Ok() {
        ToggleErrorScreen(false);
    }

    private void OnConnectedToMaster() {
        print("Connected to master");
        PhotonNetwork.automaticallySyncScene = true; //automaticament sincroniza a cena que o masterclient esta
        //PhotonNetwork.playerName = PlayerNetwork.Instance.m_playerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby() {
        print("Joined lobby");

        //if (!PhotonNetwork.inRoom) {
        //    print("Deu ruim pra entrar no lobby :(");
        //    MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling(); //seta o lobby de room no final da hierarquia, assim fica em cima de tudo
        //}

        ToggleLoadingScreen(false);
        PhotonNetwork.LoadLevel(1);
    }

    private void OnConnectionFail() {
        print("Deu ruim :(");
        ToggleErrorScreen(true);
    }

    private void OnFailedToConnectToPhoton() {
        print("Deu ruim pra conectar no photon");
        ToggleErrorScreen(true);
    }

    private void ToggleLoadingScreen(bool active) {
        m_loadingScreen.SetActive(active);
    }

    private void ToggleErrorScreen(bool active) {
        m_errorScreen.SetActive(active);
    }
}
