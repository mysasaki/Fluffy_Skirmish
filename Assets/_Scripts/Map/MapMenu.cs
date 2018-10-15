using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMenu : MonoBehaviour {
    [SerializeField] private GameObject container;

    public bool m_isActive;
    public GameObject compass;

    private void Start() {
        container.SetActive(false);
        m_isActive = false;
    }

    public void ShowMapMenu() {
        if(!m_isActive) {
            m_isActive = true;
            container.SetActive(m_isActive);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HideMapMenu() {
        if(m_isActive) {
            m_isActive = false;
            container.SetActive(m_isActive);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
