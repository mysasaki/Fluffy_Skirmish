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

    public bool moveAudioActive;

    public float delayRun;
    public float delaySprint;

    public float delayBetweenSteps;

    public bool canPlay;

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

        if (m_photonView.isMine) {
            m_photonView.RPC("PlayLocalMove", PhotonTargets.All, m_player.ID);
        } else {
            m_photonView.RPC("PlayMove", PhotonTargets.Others, m_player.ID);
        }
    }

    public void StopMoveAudio() {
        m_photonView.RPC("StopMove", PhotonTargets.All, m_player.ID);

    }

    [PunRPC]
    private void PlayDeath(int id) {
        if (id != m_player.ID)
            return;

        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = deathAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.3f;
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
    public void PlayLocalMove(int id) {
        if (id != m_player.ID)
            return;

        canPlay = true;
        StartCoroutine("PlayAudio");
    }

    IEnumerator PlayAudio() {
        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = moveAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.1f;

        while (canPlay) {
            yield return new WaitForSeconds(delayBetweenSteps);
            m_audioSource.Play();
        }
    }

    [PunRPC]
    private void StopMove(int id) {
        if (id != m_player.ID)
            return;
        canPlay = false;
        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();
        StopCoroutine("PlayAudio");
        moveAudioActive = false;
        m_audioSource.Stop();
    }

}
