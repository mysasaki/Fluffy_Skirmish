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

    private void Start() {
        InitializeAllPickups();
    }

    private void InitializeAllPickups() {
        GameObject[] healthList = GameObject.FindGameObjectsWithTag("PickupHealth");
        foreach (GameObject g in healthList) {
            PickupHealth puh = g.GetComponentInChildren<PickupHealth>();
            RespawnPickupHealth(puh.ID);
        }

        GameObject[] ammoList = GameObject.FindGameObjectsWithTag("PickupAmmo");
        foreach (GameObject g in ammoList) {
            PickupAmmo pua = g.GetComponentInChildren<PickupAmmo>();
            RespawnPickupAmmo(pua.ID);
        }

        GameObject[] pistolList = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject g in pistolList) {
            PistolTrigger p = g.GetComponentInChildren<PistolTrigger>();
            RespawnPistol(p.ID);
        }
    }

    public void RespawnPickupHealth(int id) {
        float randomZ = Random.Range(0f, 150f); //TODO: arrumar o range depois
        float randomX = Random.Range(0f, 150f);
        m_photonView.RPC("RPC_RespawnPickupHealth", PhotonTargets.All, id, randomX, randomZ);
    }

    public void RespawnPickupAmmo(int id) {
        float randomZ = Random.Range(0f, 150f);
        float randomX = Random.Range(0f, 150f);
        m_photonView.RPC("RPC_RespawnPickupAmmo", PhotonTargets.All, id, randomX, randomZ);
    }

    public void RespawnPistol(int id) {
        float randomZ = Random.Range(0f, 150f);
        float randomX = Random.Range(0f, 150f);
        m_photonView.RPC("RPC_RespawnPistol", PhotonTargets.All, id, randomX, randomZ);
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

    [PunRPC]
    private void RPC_RespawnPickupAmmo(int id, float posX, float posZ) {
        GameObject[] pickupsAmmo = GameObject.FindGameObjectsWithTag("PickupAmmo");
        foreach (GameObject g in pickupsAmmo) {
            PickupAmmo pu = g.GetComponentInChildren<PickupAmmo>();
            
            if(pu.ID == id) {
                pu.ToggleMesh(false);
                pu.RespawnAmmo(posX, posZ);
                return;
            }
        }
    }

    [PunRPC]
    private void RPC_RespawnPistol(int id, float posX, float posZ) {
        GameObject[] pistols = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject g in pistols) {
            PistolTrigger pt = g.GetComponentInChildren<PistolTrigger>();
            if(pt.ID == id) {
                pt.ToggleMesh(false);
                pt.RespawnPistol(posX, posZ);
                return;
            }
        }
    }
}
