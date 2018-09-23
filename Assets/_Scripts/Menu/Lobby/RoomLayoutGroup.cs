using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour {

    [SerializeField]
    private GameObject m_roomListingPrefab;
    private GameObject RoomListingPrefab {
        get { return m_roomListingPrefab; }
    }

    private List<RoomListing> m_roomListingButtons = new List<RoomListing>();
    private List<RoomListing> RoomListingButtons {
        get { return m_roomListingButtons; }
    }

    private void OnReceivedRoomListUpdate() { //1. callback do photon quando tem um novo roomUpdate
        RoomInfo[] rooms = PhotonNetwork.GetRoomList(); //2. Recebe todas as rooms do photonNetwork
        
        foreach (RoomInfo room in rooms) { //3. Loop pra percorrer todas as rooms
            RoomReceived(room); //4. Passa pra RoomReceived
        }

        RemoveOldRooms(); 
    }

    private void RoomReceived(RoomInfo room) { //5. verifica se a room ja existe, e atualiza o UI se necessario
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name); //percorre todos os itens de roomlistingbuttons e verifica se x.roomname == room.name

        if (index == -1) { 
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers) {
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);

                index = (RoomListingButtons.Count - 1);
            }
        }

        if (index != -1) {
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms() { //6. Remove botoes dos rooms que nao existem mais
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in RoomListingButtons) {
            if (!roomListing.Updated) {
                removeRooms.Add(roomListing);
            } else {
                roomListing.Updated = false;
            }
        }

        foreach (RoomListing roomListing in removeRooms) {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
