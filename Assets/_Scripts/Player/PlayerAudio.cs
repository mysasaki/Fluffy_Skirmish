using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    private PhotonView m_photonView;
    private AudioSource m_audioSource;
    private AudioListener m_audioListener;
    private Player m_player;

    public AudioClip deathAudio;
    public AudioClip moveAudio;
    public AudioClip pickupAudio;
    public AudioClip damageAudio;

    public bool moveAudioActive;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        m_player = GetComponent<Player>();
        m_audioSource = GetComponent<AudioSource>();
        m_audioListener = GetComponent<AudioListener>();

        moveAudioActive = false;

        if (!m_photonView.isMine)
            m_audioListener.enabled = false; //desativa o audiolistener das instancias dos outros players
    }

    public void DeathAudio() {
        m_photonView.RPC("PlayDeath", PhotonTargets.All, m_player.ID);
    }

    public void StartMoveAudio() {
        m_photonView.RPC("PlayMove", PhotonTargets.All, m_player.ID);
    }

    public void StopMoveAudio() {
        m_photonView.RPC("StopMove", PhotonTargets.All, m_player.ID);
    }

    public void TakeDamageAudio() {
        m_photonView.RPC("PlayTakeDamage", PhotonTargets.All, m_player.ID);
    }

    public void PickupAudio() {
        m_audioSource.clip = pickupAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.1f;
        m_audioSource.Play();
    }

    [PunRPC]
    private void PlayDeath(int id) {
        if (id != m_player.ID)
            return;

        if(!m_audioSource) 
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = deathAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.1f;
        m_audioSource.Play();

    }

    [PunRPC]
    private void PlayTakeDamage(int id) {
        if (id != m_player.ID)
            return;

        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = damageAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.08f;
        m_audioSource.Play();
    }

    [PunRPC]
    private void PlayMove(int id) {
        if (id != m_player.ID)
            return;

        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = moveAudio;
        m_audioSource.loop = true;
        m_audioSource.volume = 0.05f;
        m_audioSource.Play();
    }

    [PunRPC]
    private void StopMove(int id) {
        if (id != m_player.ID)
            return;

        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        moveAudioActive = false;
        m_audioSource.Stop();
    }

}
