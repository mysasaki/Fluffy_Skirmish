using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour {

    public GameObject container;
    public bool m_isActive = false;


    private void Awake() {
        HideEscape();
    }

    public void Toggle() {
        m_isActive = !m_isActive;

        if (m_isActive)
            ShowEscape();
        else
            HideEscape();
    }

    private void ShowEscape() {
        container.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideEscape() {
        container.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
