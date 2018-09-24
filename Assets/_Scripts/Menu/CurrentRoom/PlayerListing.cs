using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {

    public PhotonPlayer m_photonPlayer { get; private set; }

    [SerializeField]
    private Text m_playerName;
    private Text PlayerName {
        get { return m_playerName; }
    }

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer) {

        m_photonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;
    }
}
