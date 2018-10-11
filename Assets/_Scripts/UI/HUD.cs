using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    private bool m_isActive = true;
    public GameObject container;

    public void HideUI() {
        if (!m_isActive)
            return;

        m_isActive = false;
        container.SetActive(m_isActive);
    }

    public void ShowUI() {
        if (m_isActive)
            return;

        m_isActive = true;
        container.SetActive(m_isActive);
    }
}
