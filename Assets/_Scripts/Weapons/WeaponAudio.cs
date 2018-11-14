using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudio : MonoBehaviour {

    private PhotonView m_photonView;
    private AudioSource m_audioSource;

    private int ID;

    public AudioClip fireAudio;
    public AudioClip reloadAudio;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        m_audioSource = GetComponent<AudioSource>();
        ID = m_photonView.ownerId;
    }

    public void FireAudio() {
        m_photonView.RPC("PlayFire", PhotonTargets.All, this.ID);
    }

    public void ReloadAudio() {
        m_photonView.RPC("PlayReload", PhotonTargets.All, this.ID);
    }

    [PunRPC]
    private void PlayFire(int id) {
        if (id != this.ID)
            return;

        if(!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = fireAudio;
        m_audioSource.Play();
    }

    [PunRPC]
    private void PlayReload(int id) {
        if (id != this.ID)
            return;

        if(!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = reloadAudio;
        m_audioSource.Play();
    }
}
