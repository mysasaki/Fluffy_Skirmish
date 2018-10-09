using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Atalhos pra testar as coisa
public class PlayerDebug : MonoBehaviour {

    private Player m_player;

    private void Awake() {
        m_player = GetComponent<Player>();
    }

    private void FixedUpdate() {
        if(Input.GetButtonDown("DebugKill")) {
            PlayerManagement.Instance.DebugKill(m_player.ID);
        }
    }
}
