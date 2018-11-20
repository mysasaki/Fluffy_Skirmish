using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas : MonoBehaviour {

    [SerializeField]
    private GameObject m_creditsScreen;
    private GameObject creditsScreen {
        get { return m_creditsScreen; }
    }

    [SerializeField]
    private GameObject m_mainScreen;
    private GameObject mainScreen {
        get { return m_mainScreen; }
    }

    private void Start() {
        //m_mainScreen.transform.SetAsLastSibling();
        m_creditsScreen.SetActive(false);
    }

    public void OnClick_Controls() {
        print("control");
        m_creditsScreen.SetActive(true);
        m_mainScreen.SetActive(false);
    }

    public void OnClick_Back() {
        m_creditsScreen.SetActive(false);
        m_mainScreen.SetActive(true);
    }

}
