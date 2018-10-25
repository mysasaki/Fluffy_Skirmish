using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script responsavel em administrar todos os canvas e os filhos
public class MainCanvasManager : MonoBehaviour {

    public static MainCanvasManager Instance;

    [SerializeField]
    private LobbyCanvas m_lobbyCanvas;
    public LobbyCanvas LobbyCanvas {
        get { return m_lobbyCanvas; }
    }

    [SerializeField]
    private CurrentRoomCanvas m_currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas {
        get { return m_currentRoomCanvas; }
    }

    private void Awake() {
        Instance = this;
    }
}
