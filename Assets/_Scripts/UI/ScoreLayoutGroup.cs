using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLayoutGroup : MonoBehaviour {

    public GameObject scoreListingPrefab;
    public bool m_isActive;

    private List<PlayerStats> m_playerStats = new List<PlayerStats>();
    //private List<GameObject> m_scoreObjectList = new List<GameObject>();
    private List<ScoreListing> m_scoreListingList = new List<ScoreListing>();

    public void InitializeScoreboard() {
        print("Initialize scoreboard");
        foreach (PlayerStats p in m_playerStats) {

            int index = m_scoreListingList.FindIndex(x => x.id == p.ID);

            if (index == -1) {  //nao permite que players repetidos entre na lista
                AddScore(p);
            }
        }
    }

    public void UpdateList() {
        print("Update scoreboard");
        m_playerStats = PlayerManagement.Instance.m_playerStatsList;

        if (m_scoreListingList.Count == 0)
            InitializeScoreboard();
    }

    public void UpdateScoreboard() {
        UpdateList();
        m_playerStats.Sort((p1, p2) => p1.Kills.CompareTo(p2.Kills));

        foreach (PlayerStats p in m_playerStats) {
            int index = m_scoreListingList.FindIndex(x => x.id == p.ID);
            
            if(index != -1) {
                ScoreListing score = m_scoreListingList[index];
                score.UpdateText(p.Kills.ToString(), p.Death.ToString());

            } else {
                AddScore(p);
                UpdateScoreboard();
                return;
            }
        }
    }

    private void AddScore(PlayerStats p) { //deve acontecer se um player entrar no meio da partida
        GameObject obj = Instantiate(scoreListingPrefab);
        ScoreListing score = obj.GetComponent<ScoreListing>();
        score.InitializeScore(p.ID, p.Name, p.Kills.ToString(), p.Death.ToString());
        m_scoreListingList.Add(score);
        //m_scoreObjectList.Add(obj);
        obj.transform.SetParent(transform, false);
    }


}
