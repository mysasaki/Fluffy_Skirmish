using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMenu : MonoBehaviour {
    [SerializeField] private GameObject container;
    [SerializeField] private ClosureManager closureManager;

    public bool m_isActive;
    public GameObject compass;

    public List<int> idsToOpen = new List<int>();
    public List<int> idsClosed = new List<int>();
    public List<int> idsToClose = new List<int>(); //precisa armazenar pra poder atualizar no minimapa. Nao da pra fazer direto pois vai tentar acessar um objecto desativado

    private void Start() {
        container.SetActive(false);
        m_isActive = false;
    }

    public void ShowMapMenu() {
        if(!m_isActive) {
            m_isActive = true;
            container.SetActive(m_isActive);
            UpdateClosures();
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

    public void UpdateClosures() {
        print("Update closures");
        closureManager.Open(idsToOpen);      
        
        closureManager.Closed(idsClosed);
        closureManager.ToClose(idsToClose);
    }

    public void ToClose(List<int> ids) {
        idsToClose = ids;

        if(closureManager) {
            UpdateClosures();
        }
    }

    public void Close(List<int> ids) {
        idsClosed = ids;

        if(closureManager) {
            UpdateClosures();
        }
    }

    public void Open(List<int> ids) {
        idsToOpen = ids;

        if (closureManager) {
            UpdateClosures();
        }
    }
}
