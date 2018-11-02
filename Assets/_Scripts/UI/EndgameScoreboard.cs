using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgameScoreboard : MonoBehaviour {

    public GameObject scoreListingPrefab;

    private List<PlayerStats> m_playerStats = new List<PlayerStats>();
    private List<ScoreListing> m_scoreListing = new List<ScoreListing>();

    private void Awake() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_playerStats = PlayerManagement.Instance.m_playerStatsList;
        m_playerStats.Sort(SortByScore);

        foreach (PlayerStats p in m_playerStats) {
            int index = m_scoreListing.FindIndex(x => x.id == p.ID);
            if(index == -1) {
                AddScore(p);
            }
        }
    }

    private void AddScore(PlayerStats p) {
        GameObject obj = Instantiate(scoreListingPrefab);
        ScoreListing score = obj.GetComponent<ScoreListing>();
        score.InitializeScore(p.ID, p.Name, p.Kills.ToString(), p.Death.ToString());
        m_scoreListing.Add(score);
        obj.transform.SetParent(transform, false);
    }

    private static int SortByScore(PlayerStats p1, PlayerStats p2) {
        return p1.Kills.CompareTo(p2.Kills);
    }
}
