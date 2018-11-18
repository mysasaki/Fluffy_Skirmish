using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {

    public static PickupManager Instance;

    private PhotonView m_photonView;

    private void Awake() {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    public void GetPickupHealth(int id) {
        float randomZ = Random.Range(0f, 150f); //TODO: arrumar o range depois
        float randomX = Random.Range(0f, 150f);
        m_photonView.RPC("RPC_RespawnPickupHealth", PhotonTargets.All, id, randomX, randomZ);
    }
    
    [PunRPC]
    private void RPC_RespawnPickupHealth(int id, float posX, float posZ) {

        GameObject[] pickupsHealth = GameObject.FindGameObjectsWithTag("PickupHealth");
        foreach (GameObject g in pickupsHealth) {
            PickupHealth pu = g.GetComponentInChildren<PickupHealth>();
            
            if(pu.ID == id) {
                pu.ToggleMesh(false);
                pu.RespawnHealth(posX, posZ);
                return;
            }
        }
    }
}
