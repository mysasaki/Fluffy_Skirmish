using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour {

    public AudioSource bgmAudioSource;
    private bool m_isActive;

    private void Start() {
        m_isActive = true;
    }

    public void ToggleAudio() {
        m_isActive = !m_isActive;

        if (m_isActive)
            bgmAudioSource.Play();
        else
            bgmAudioSource.Stop();
    }
}
