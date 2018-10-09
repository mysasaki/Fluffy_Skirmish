using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Vai ficar responsavel pelas coisas locais. Atualizaçao do hud
//Comunica diretamente com o playermanagement pra fazer sync dos player stats (health, ammo, kill/death)
public class GameManager : MonoBehaviour { 

    public static GameManager Instance = null;
    
    private HUD m_hud {
        get { return FindObjectOfType<HUD>(); }
        set { m_hud = value; }
    }

    private Escape m_escape {
        get { return FindObjectOfType<Escape>(); }
        set { m_escape = value; }
    }

    private Respawn m_respawn {
        get { return FindObjectOfType<Respawn>(); }
        set { m_respawn = value; }
    }

    private PlayerWeapon m_playerWeapon;
    private PlayerInput m_playerInput;
    private Player m_player;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        GetPlayerComponents();
        
    }

    private void Update() {
        if (m_player == null)
            GetPlayerComponents();

        if(m_playerInput)
            if(m_hud)
                if(m_playerWeapon)
                    m_hud.UpdateUI(m_playerWeapon, m_player);
    }

    private void GetPlayerComponents() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0) {
            foreach (GameObject p in players) {
                PhotonView photonView = p.GetComponent<PhotonView>();

                if (photonView.isMine) {
                    m_player = p.GetComponent<Player>();
                    m_playerInput = p.GetComponent<PlayerInput>();
                    m_playerWeapon = p.GetComponent<PlayerWeapon>();
                    return;
                }
            }
        }
    }

    public void ToggleEsc() {
        m_escape.Toggle();
    }

    public void StartRespawn() {
        m_respawn.StartRespawn();
    }
}

