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
    public bool Respawning = false;
    public float RespawnTime = 10;
    public GameObject Mesh;

    private PhotonView m_photonView;

    public PlayerAim playerAim;

    public Ragdoll ragdoll;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        ragdoll = GetComponentInChildren<Ragdoll>(); //ref do ragdoll
    }

    private void Start() {
        if(m_photonView.isMine) {
            m_photonView.RPC("RPC_UpdatePlayerData", PhotonTargets.Others, ID, Name);
        }
    }

    public void Update() {
        if (!m_photonView.isMine)
            return;

        if (IsDead) {
            print("Player " + Name + " diededdened.");
            ragdoll.active = true; //ativa o ragdoll
            IsDead = false;
            StartCoroutine(StartRespawnPlayer());
            GameManager.Instance.StartRespawn();
        }
    }

    private IEnumerator StartRespawnPlayer() {
        print("startRespawnPlayer called");
        yield return new WaitForSeconds(RespawnTime);
        ToggleMesh(false);
        RespawnPlayer();
    }

    private void RespawnPlayer() {
        ragdoll.active = false; //desativa o ragdoll
        Respawning = true;
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

    public void ToggleMesh(bool activate) {
        Mesh.SetActive(activate);
    }

    public void FinishRespawn() {
        this.IsDead = false;
        this.Respawning = false;
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
