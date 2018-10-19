using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour {

    [SerializeField]
    private GameObject m_playerListingPrefab;
    private GameObject PlayerListingPrefab {
        get { return m_playerListingPrefab; }
    }

    [SerializeField]
    private Text m_roomState;
    private Text roomState {
        get { return m_roomState; }
    }

    private List<PlayerListing> m_playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings {
        get { return m_playerListings; }
    }


    //Callback do photon. Quando o masterclient sai do jogo, esse callback vai pra todos os clientes
    //Kicka todos os players da sala quando o master sai da sala. Reabilitar caso precisar
    /*private void OnMasterClientSwitched(PhotonPlayer newMasterClient) {
        PhotonNetwork.LeaveRoom();
    }*/

    //Callback do photon quando voce entra numa room
    private void OnJoinedRoom() {

        foreach (Transform  child in transform) {
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling(); //Vai colocar no fim da hierarquia. ie. vai colocar por cima do lobby

        //Add todos os current players
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;

        for (int i = 0; i < photonPlayers.Length; i++) {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    //Callback do photon quando alguem entra na sala
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer) {
        PlayerJoinedRoom(photonPlayer);
    }

    //CallBack do photon quando player sai da sala
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
        PlayerLeftRoom(photonPlayer);
    }

    private void PlayerJoinedRoom(PhotonPlayer photonPlayer) {
        if (photonPlayer == null) {
            return;
        }

        PlayerLeftRoom(photonPlayer); //Evita ter players duplicados na sala. Acontece as vezes por causa da internets

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer); //seta o texto do prefab

        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(PhotonPlayer photonPlayer) {
        int index = PlayerListings.FindIndex(x => x.m_photonPlayer == photonPlayer);

        if (index != -1) { //encontro o player na lista
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public void OnClick_RoomState() {

        if (!PhotonNetwork.isMasterClient)  //Caso nao seja a pessoa que criou a room, return
            return;

        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;

        m_roomState.text = (PhotonNetwork.room.IsOpen) ? "Lock room" : "Unlock room";
    }

    public void OnClick_LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}
