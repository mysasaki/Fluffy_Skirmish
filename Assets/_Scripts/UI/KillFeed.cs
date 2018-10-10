using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour {

    public GameObject KillListingPrefab;
    public int maxCapacity = 3;

    private List<Kill> m_killList = new List<Kill>();

    public void AddKill(Kill newKill, int sender_id) {
        print("SNEDER ID " + sender_id + ". MY ID " + PhotonNetwork.player.ID);
        if (sender_id != PhotonNetwork.player.ID)
            return;

        if(m_killList.Count >= maxCapacity) {
            m_killList.RemoveAt(0);
        }

        m_killList.Add(newKill);
        GameObject killObj = Instantiate(KillListingPrefab);
        KillListing killListing = killObj.GetComponent<KillListing>();
        killListing.UpdateText(newKill.Killer + " > pewpew > " + newKill.Victim);
        killObj.transform.SetParent(transform, false);
    }

}

public class Kill {
    public string Killer;
    public string Victim;
    //public string Weapon;

    public Kill() {
        this.Killer = "";
        this.Victim = "";
    }
 }
