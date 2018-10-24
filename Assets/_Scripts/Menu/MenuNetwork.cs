﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNetwork : MonoBehaviour {

    [SerializeField]
    private GameObject m_loadingScreen;
    private GameObject loadingScreen {
        get { return m_loadingScreen; }
    }

    [SerializeField]
    private GameObject m_connectionError;
    private GameObject connectionError {
        get { return m_connectionError; }
    }

    [SerializeField]
    private GameObject m_nameError;
    private GameObject nameError {
        get { return m_nameError; }
    }

    [SerializeField]
    private Text m_playerName;
    private Text playerName {
        get { return m_playerName; }
    }

    public void OnClick_Play() {

        if(playerName.text.Length > 0) {
            if (!PhotonNetwork.connected) {
                print("Connecting to server");
                PhotonNetwork.ConnectUsingSettings("0.1"); //conecta ao servidor

                ToggleLoadingScreen(true);
            }

        } else {
            ToggleNameError(true);
        }
    }

    public void OnClick_Ok() {
        ToggleConnectionError(false);
        ToggleNameError(false);
    }

    private void OnConnectedToMaster() {
        print("Connected to master");
        PhotonNetwork.automaticallySyncScene = true; //automaticament sincroniza a cena que o masterclient esta
        PhotonNetwork.playerName = playerName.text;
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
        ToggleConnectionError(true);
    }

    private void OnFailedToConnectToPhoton() {
        print("Deu ruim pra conectar no photon");
        ToggleConnectionError(true);
    }

    private void ToggleLoadingScreen(bool active) {
        m_loadingScreen.SetActive(active);
    }

    private void ToggleConnectionError(bool active) {
        connectionError.SetActive(active);
    }

    private void ToggleNameError(bool active) {
        nameError.SetActive(active);
    }
}
