using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {
    /*
    private void Start() {
        if (!PhotonNetwork.connected) { 
            print("Connecting to server");
            PhotonNetwork.ConnectUsingSettings("0.1"); //conecta ao servidor
        }
	}*/

    private void OnConnectedToMaster() {
        print("Connected to master from lobby");
        PhotonNetwork.automaticallySyncScene = true; //automaticament sincroniza a cena que o masterclient esta
        //PhotonNetwork.playerName = "Player#" + Random.Range(1000, 9999);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    /*
    private void OnJoinedLobby() {
        print("Joined lobby");

        if (!PhotonNetwork.inRoom) {
            MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling(); //seta o lobby de room no final da hierarquia, assim fica em cima de tudo
        }     
    }*/
}
