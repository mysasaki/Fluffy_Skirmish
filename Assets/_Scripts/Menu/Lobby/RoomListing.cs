using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour {

    [SerializeField]
    private Text m_roomNameText;
    private Text RoomNameText {
        get { return m_roomNameText; }
    }

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

	private void Start () {
        //listener
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        if (lobbyCanvasObj == null) {
            return;
        }

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClick_JoinRoom(RoomNameText.text));

	}

    private void OnDestroy() {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text) {
        RoomName = text;
        RoomNameText.text = RoomName;
    }
}
