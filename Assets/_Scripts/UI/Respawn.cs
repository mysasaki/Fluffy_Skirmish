using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour {
    public Slider slider;
    public Text time;
    public GameObject container;

    public bool m_isActive = false;
    private float m_countingValue = 10;

    private void Awake() {
        this.time.text = "10";
        this.slider.value = 10;
        m_isActive = false;
        container.SetActive(false);
    }

    private void setTimeValues() {
        this.time.text = m_countingValue.ToString();
        this.slider.value = Mathf.Round(m_countingValue);
    }

    public void StartRespawn() {
        m_countingValue = 10;
        m_isActive = true;
        container.SetActive(true);
        StartCoroutine(StartCountRespawn(10));
    }

    private IEnumerator StartCountRespawn(float countingValue) {
        this.m_countingValue = countingValue;

        while(m_countingValue > 0) {
            yield return new WaitForSeconds(1.0f);
            m_countingValue -= 1;
            setTimeValues();
        }

        EndRespawn();
    }

    public void EndRespawn() {
        m_isActive = false;
        container.SetActive(false);    
    }

}
