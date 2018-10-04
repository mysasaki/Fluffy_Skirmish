using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Vai ficar responsavel pelas coisas locais. Atualizaçao do hud
//Comunica diretamente com o playermanagement pra fazer sync dos player stats (health, ammo, kill/death)
public class GameManager : MonoBehaviour { 

    public static GameManager Instance = null;
    
    private PlayerUI m_playerUI {
        get { return FindObjectOfType<PlayerUI>(); }
        set { m_playerUI = value; }
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
        UpdateUI();
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

    private void UpdateUI() {
        if (m_playerInput) {
            if (m_playerUI) {
                if (m_playerWeapon) {

                    if (m_playerWeapon.currentWeapon == null) {
                        m_playerUI.ammoText.text = "Unarmed";
                        
                    } else {
                        m_playerUI.ammoText.text = m_playerWeapon.currentWeapon.ammo.clipAmmo + "/" + m_player.Ammo;
                    }
                }

                if (m_playerUI.healthBar && m_playerUI.healthText) {
                    m_playerUI.healthBar.value = Mathf.Round(m_player.Health);
                    m_playerUI.healthText.text = Mathf.Round(m_playerUI.healthBar.value).ToString();
                }
            }

        }
    }
}

