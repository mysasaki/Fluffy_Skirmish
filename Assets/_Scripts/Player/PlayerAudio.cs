using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    public AudioSource m_audioSource;
    public AudioSource m_stepAudioSource;

    private PhotonView m_photonView;
    private AudioListener m_audioListener;
    private Player m_player;

    public AudioClip deathAudio;
    public AudioClip moveAudio;
    public AudioClip pickupAudio;
    public AudioClip damageAudio;

    public bool moveAudioActive;

    public float delayRun;
    public float delaySprint;
    public float delayBetweenSteps;

    public bool canPlay;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        m_player = GetComponent<Player>();
        m_audioListener = GetComponent<AudioListener>();

        moveAudioActive = false;

        if (!m_photonView.isMine)
            m_audioListener.enabled = false; //desativa o audiolistener das instancias dos outros players
    }

    public void DeathAudio() {
        m_photonView.RPC("PlayDeath", PhotonTargets.All, m_player.ID);
    }

    public void StartMoveAudio() {
        PlayLocal(m_player.ID);
        m_photonView.RPC("PlayMove", PhotonTargets.Others, m_player.ID);
        /*if (m_photonView.isMine) {
            m_photonView.RPC("PlayLocalMove", PhotonTargets.All, m_player.ID);
        } else {
            m_photonView.RPC("PlayMove", PhotonTargets.All, m_player.ID);
        }*/
    }

    public void StopMoveAudio() {
        m_photonView.RPC("StopMove", PhotonTargets.All, m_player.ID);
    }

    public void TakeDamageAudio() {
        m_photonView.RPC("PlayTakeDamage", PhotonTargets.All, m_player.ID);
    }

    public void PickupAudio() {
        float pitch = Random.Range(0.5f, 1.5f);
        m_audioSource.clip = pickupAudio;
        m_audioSource.loop = false;
        m_audioSource.volume = 0.1f;
        m_audioSource.pitch = pitch;
        m_audioSource.Play();
    }

    private void PlayLocal(int id) {
        if (id != m_player.ID)
            return;

        canPlay = true;
        StartCoroutine("PlayAudio");
    }

    [PunRPC]
    private void PlayDeath(int id) {
        if (id != m_player.ID)
            return;

        if (!m_audioSource)
            m_audioSource = GetComponent<AudioSource>();

        m_audioSource.clip = deathAudio;
        m_audioSource.loop = false;
        m_audioSource.pitch = 1.0f;
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
        m_audioSource.pitch = 1.0f;
        m_audioSource.volume = 0.08f;
        m_audioSource.Play();
    }

    [PunRPC]
    private void PlayMove(int id) {
        if (id != m_player.ID)
            return;

        m_stepAudioSource.clip = moveAudio;
        m_stepAudioSource.loop = true;
        m_stepAudioSource.pitch = 1.0f;
        m_stepAudioSource.volume = 0.1f;
        m_stepAudioSource.Play();

    }

    [PunRPC]
    public void PlayLocalMove(int id) {
        if (id != m_player.ID)
            return;

        canPlay = true;
        StartCoroutine("PlayAudio");
    }

    IEnumerator PlayAudio() {
        m_stepAudioSource.clip = moveAudio;
        m_stepAudioSource.loop = false;
        m_stepAudioSource.volume = 0.1f;

        while (canPlay) {
            yield return new WaitForSeconds(delayBetweenSteps);
            m_stepAudioSource.Play();
        }
    }

    [PunRPC]
    private void StopMove(int id) {
        if (id != m_player.ID)
            return;
        canPlay = false;
        StopCoroutine("PlayAudio");
        moveAudioActive = false;
        m_stepAudioSource.Stop();
    }

}
