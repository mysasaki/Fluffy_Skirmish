using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Vai ficar responsavel pelas coisas locais. Atualizaçao do hud
//Comunica diretamente com o playermanagement pra fazer sync dos player stats (health, ammo, kill/death)
public class GameManager : MonoBehaviour { 

    public static GameManager Instance = null;

    private PlayerInput m_playerInput {
        get { return FindObjectOfType<PlayerInput>(); }
        set { m_playerInput = value; }
    }

    private PlayerUI m_playerUI {
        get { return FindObjectOfType<PlayerUI>(); }
        set { m_playerUI = value; }
    }

    private PlayerWeapon m_weaponHandler {
        get { return FindObjectOfType<PlayerWeapon>(); }
        set { m_weaponHandler = value; }
    }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        UpdateUI();
    }

    private void UpdateUI() {
        if (m_playerInput) {
            if (m_playerUI) {
                if (m_weaponHandler) {

                    if (m_weaponHandler.currentWeapon == null) {
                        m_playerUI.ammoText.text = "Unarmed";

                    } else {
                        m_playerUI.ammoText.text = m_weaponHandler.currentWeapon.ammo.clipAmmo + "//" + m_weaponHandler.currentWeapon.ammo.carryingAmmo;
                    }
                }

                if (m_playerUI.healthBar && m_playerUI.healthText) {
                    m_playerUI.healthText.text = Mathf.Round(m_playerUI.healthBar.value).ToString();
                }
            }

        }
    }
}
