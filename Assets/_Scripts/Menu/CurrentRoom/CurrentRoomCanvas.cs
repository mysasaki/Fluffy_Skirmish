using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour {

    public void OnClick_StartSync() {
        if (!PhotonNetwork.isMasterClient) //só o masterclient consegue iniciar a partida
            return;

        PhotonNetwork.LoadLevel(1);
    }

    public void OnClick_StartDelayed() {

        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = false; //impede pessoas entrarem no jogo enquanto ta rolando
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }
}
