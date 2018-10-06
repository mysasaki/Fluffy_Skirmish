using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int ID;
    public string Name;
    public int Health = 100;
    public int Ammo = 24;
    public int Kill = 0;
    public int Death = 0;
    public bool IsDead = false;

    private PhotonView m_photonView;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        if(m_photonView.isMine) {
            m_photonView.RPC("RPC_UpdatePlayerData", PhotonTargets.Others, ID, Name);
        }
    }

    public void Update() {
        if (IsDead) {
            print("Player " + Name + " diededdened.");
            IsDead = false;
            StartCoroutine(StartRespawnPlayer());
        }
    }

    private IEnumerator StartRespawnPlayer() {
        print("startRespawnPlayer called");
        yield return new WaitForSeconds(10);
        gameObject.SetActive(false);
        RespawnPlayer();      
    }

    private void RespawnPlayer() {
        PlayerManagement.Instance.RespawnPlayer(this.ID);
    }

    public void UpdateHealth(int health) {
        this.Health = health;
    }

    public void UpdateAmmo(int ammo) {
        this.Ammo = ammo;
    }

    public void UpdateDeath(int death) {
        this.Death = death;       
    }

    public void UpdateKill(int kill) {
        this.Kill = kill;
    }

    [PunRPC]
    private void RPC_UpdatePlayerData(int ID, string name) {
        if (m_photonView.isMine)
            return;
        
        if(m_photonView.owner.ID == ID) {
            this.ID = ID;
            this.Name = name;
        }
    }
}
