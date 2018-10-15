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

    private Scoreboard m_scoreboard {
        get { return FindObjectOfType<Scoreboard>(); }
        set { m_scoreboard = value; }
    }

    private Minimap m_mapmenu {
        get { return FindObjectOfType<Minimap>(); }
        set { m_mapmenu = value; }
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
            if(m_playerUI)
                if(m_playerWeapon)
                    m_playerUI.UpdateUI(m_playerWeapon, m_player);
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

    #region Escape
    public void ToggleEsc() {
        m_escape.Toggle();
    }

    public bool IsEscapeActive() {
        return m_escape.m_isActive;
    }
    #endregion

    #region Respawn & hud
    public void StartRespawn() {
        m_respawn.StartRespawn();
    }

    public bool IsRespawnActive() {
        return m_respawn.m_isActive;
    }

    public void HideHUD() {
        m_hud.HideUI();
    }

    public void ShowHUD() {
        m_hud.ShowUI();
    }

    public bool IsHUDActive() {
        return m_hud.m_isActive;
    }
    #endregion

    #region Scoreboard
    public void ShowScoreboard() {
        m_scoreboard.ShowScoreboard();
    }

    public void HideScoreboard() {
        m_scoreboard.HideScoreboard();
    }

    public bool IsScoreboardActive() {
        return m_scoreboard.m_isActive;
    }
    #endregion

    #region Map
    public void ShowMap() {
        m_mapmenu.ShowMapMenu();
    }

    public void HideMap() {
        m_mapmenu.HideMapMenu();
    }

    public bool IsMapActive() {
        return m_mapmenu.m_isActive;
    }
    #endregion
}

