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

    public PlayerAim playerAim;
    public Ragdoll ragdoll;

    private PhotonView m_photonView;
    private PlayerAudio m_playerAudio;
    private GameObject canvas;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        ragdoll = GetComponentInChildren<Ragdoll>(); //ref do ragdoll
        m_playerAudio = GetComponent<PlayerAudio>();
        canvas = GameObject.Find("Canvas");
    }

    private void Start() {
        if(m_photonView.isMine) {
            m_photonView.RPC("RPC_UpdatePlayerData", PhotonTargets.Others, ID, Name);
        }
    }

    public void Update() {
        /*if (!m_photonView.isMine)
            return;*/

        if (IsDead) {
            IsDead = false;
            Respawning = true;
            ragdoll.active = true; //ativa o ragdoll             
            //ToggleMesh(false);
            StartCoroutine(StartRespawnPlayer());

            if (m_photonView.isMine) {
                m_playerAudio.DeathAudio();
                
                GameManager.Instance.StartRespawn();
            }
            print("Player " + Name + " diededdened.");
                     
        } 
    }

    private void ToggleFalse() {
        ToggleMesh(false);
    }

    private IEnumerator StartRespawnPlayer() {
        Invoke("ToggleFalse", 5);
        yield return new WaitForSeconds(RespawnTime);
        ToggleMesh(true);

        if (m_photonView.isMine)
            RespawnPlayer();
    }

    private void RespawnPlayer() {
        //ragdoll.active = false; //desativa o ragdoll
        
        PlayerManagement.Instance.RespawnPlayer(this.ID);
        
    }


    public void UpdateHealth(int health) {
        if (this.Health > health) { //recebeu dano
            if (m_photonView.isMine) {
                DeathScreen ds = canvas.GetComponent<DeathScreen>();
                ds.ActivateDeathScreen();
                m_playerAudio.TakeDamageAudio();
            }
        } 

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
        ToggleMesh(true);
        this.IsDead = false;
        this.Respawning = false;
        this.ragdoll.active = false;
       
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
