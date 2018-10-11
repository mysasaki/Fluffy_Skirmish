using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    public ScoreLayoutGroup scoreLayoutGroup;
    public GameObject container;

    private bool m_isActive;
    //private List<PlayerStats> m_playerList = new List<PlayerStats>();

    public void Awake() {
        scoreLayoutGroup.UpdateScoreboard();
        this.m_isActive = false;
        this.container.SetActive(m_isActive);
    }

    public void ShowScoreboard() {
        if (!m_isActive) {
            this.m_isActive = true;
            this.container.SetActive(m_isActive);
        }
        scoreLayoutGroup.UpdateScoreboard();
    }

    public void HideScoreboard() {
        if (!m_isActive)
            return;

        this.m_isActive = false;
        this.container.SetActive(m_isActive);
    }
}
