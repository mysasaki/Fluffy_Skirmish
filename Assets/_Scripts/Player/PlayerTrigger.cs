using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour {

    public bool m_isHelperActive;

    private PhotonView m_photonView;

    private void Start() {
        m_isHelperActive = false;
        m_photonView = GetComponentInParent<PhotonView>();
    }

    public void OnTriggerEnter(Collider other) {
        if (!m_photonView.isMine)
            return;

        if (other.tag == "Weapon") {
            GameObject go = other.gameObject;
            WeaponTakeover wt = go.GetComponent<WeaponTakeover>();
            if (!wt.m_hasOwner) {
                if (!m_isHelperActive) {
                    m_isHelperActive = true;
                    GameManager.Instance.ShowHelper();
                }

            }
        }
    }

    public void OnTriggerExit(Collider other) {
        if (!m_photonView.isMine)
            return;

        if (m_isHelperActive) {
            m_isHelperActive = false;
            GameManager.Instance.HideHelper();
        }
    }
}
