using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas : MonoBehaviour {

    [SerializeField]
    private GameObject m_controlScreen;
    private GameObject controlScreen {
        get { return m_controlScreen; }
    }

    [SerializeField]
    private GameObject m_mainScreen;
    private GameObject mainScreen {
        get { return m_mainScreen; }
    }

    private void Start() {
        //m_mainScreen.transform.SetAsLastSibling();
        m_controlScreen.SetActive(false);
    }

    public void OnClick_Controls() {
        print("control");
        m_controlScreen.SetActive(true);
        m_mainScreen.SetActive(false);
    }

    public void OnClick_Back() {
        m_controlScreen.SetActive(false);
        m_mainScreen.SetActive(true);
    }

}
