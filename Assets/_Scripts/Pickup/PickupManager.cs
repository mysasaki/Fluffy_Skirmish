using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {

    public static PickupManager Instance;

    private List<PickupHealth> listPickupHealth = new List<PickupHealth>();

    private PhotonView m_photonView;

    private void Awake() {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {  //Passa os game object pra lsita
        GameObject[] go = GameObject.FindGameObjectsWithTag("PickupHealth");

        foreach (GameObject g in go) {
            PickupHealth pu = g.GetComponent<PickupHealth>();
            listPickupHealth.Add(pu);
        }
    }

    public void GetPickupHealth(int id) {
        float randomZ = Random.Range(0f, 150f); //TODO: arrumar o range depois
        float randomX = Random.Range(0f, 150f);
        m_photonView.RPC("RPC_RespawnPickupHealth", PhotonTargets.All, id, randomX, randomZ);
    }
    
    [PunRPC]
    private void RPC_RespawnPickupHealth(int id, float posX, float posZ) {
        int index = listPickupHealth.FindIndex(x => x.ID == id);

        if(index != -1) {
            PickupHealth puHealth = listPickupHealth[index];
            puHealth.ToggleMesh(false);
            puHealth.RespawnHealth(posX, posZ);
        }
    }
}
