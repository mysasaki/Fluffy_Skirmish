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
        foreach (PlayerStats p in m_playerStats) {

            int index = m_scoreListingList.FindIndex(x => x.id == p.ID);

            if (index == -1) {  //nao permite que players repetidos entre na lista
                AddScore(p);
            }
        }
    }

    public void UpdateList() {
        m_playerStats = PlayerManagement.Instance.m_playerStatsList;

        if (m_scoreListingList.Count == 0)
            InitializeScoreboard();
    }

    public void UpdateScoreboard() {
        UpdateList();
        m_playerStats.Sort(SortByScore);

        if (m_playerStats.Count < m_scoreListingList.Count) { //Algum fdp saiu da sala
            foreach (ScoreListing s in m_scoreListingList) {
                print(s.playerText.text);
                int index = m_playerStats.FindIndex(x => x.Name == s.playerText.text);

                if(index == -1) {
                    DeleteScore(s);
                }
            }
        }

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

    private void DeleteScore(ScoreListing s) {
        m_scoreListingList.Remove(s);
    }

    private void AddScore(PlayerStats p) { //deve acontecer se um player entrar no meio da partida
        GameObject obj = Instantiate(scoreListingPrefab);
        ScoreListing score = obj.GetComponent<ScoreListing>();
        score.InitializeScore(p.ID, p.Name, p.Kills.ToString(), p.Death.ToString());
        m_scoreListingList.Add(score);
        //m_scoreObjectList.Add(obj);
        obj.transform.SetParent(transform, false);
    }

    private static int SortByScore(PlayerStats p1, PlayerStats p2) {
        return p1.Kills.CompareTo(p2.Kills);
    }
}
