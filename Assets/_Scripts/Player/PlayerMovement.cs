using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Photon.MonoBehaviour {

    private PhotonView m_photonView;
    private Vector3 m_targetPosition;
    private Quaternion m_targetRotation;
    private PlayerAnimation m_playerAnimation;
    private Player m_player;

    public float m_moveSpeed = 10;
    public float m_runSpeed = 20;
    public bool m_sprint;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        m_playerAnimation = GetComponent<PlayerAnimation>();
        m_player = GetComponent<Player>();
    }

    private void FixedUpdate() {
        if (!m_photonView.isMine)
           SmoothMove();
    } 

    private bool IsRespawning() {
        List<PlayerStats> playerStats = PlayerManagement.Instance.m_playerStatsList;
        int index = playerStats.FindIndex(x => x.ID == this.m_player.ID);

        if (index != -1) {
            return (playerStats[index].IsDead);
        } else {
            return false;
        }
    }

    //photon callback. Called everytime you receive a package
    //Only will be called if youre observing the script
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //check if we're sending the data
        if (stream.isWriting == true) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(Health);

        } else {
            m_targetPosition = (Vector3) stream.ReceiveNext(); //pulled first entry of stream
            m_targetRotation = (Quaternion)stream.ReceiveNext();
            //Health = (float)stream.ReceiveNext();
        }
    }

    private void SmoothMove() {
        /*int index = PlayerManagement.Instance.m_playerStatsList.FindIndex(x => x.ID == m_player.ID);
        if (index == -1)
            return;

        PlayerStats playerStats = PlayerManagement.Instance.m_playerStatsList[index];*/

        if (!m_player.Respawning) {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, 0.25f); //the higher the value, more torwards the target the move is
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, 500 * Time.deltaTime);

        } else {
            transform.position = m_targetPosition;
            transform.rotation = m_targetRotation;
            m_player.Respawning = true;
        }
    }

    public void Move(float vertical, float horizontal) { //Handle player movement

        Vector3 moveVertical = transform.forward * vertical;
        Vector3 moveHorizontal = transform.right * horizontal;

        if(!m_sprint)
            transform.position += (moveHorizontal + moveVertical) * (m_moveSpeed * Time.deltaTime);           
        else
            transform.position += (moveHorizontal + moveVertical) * (m_runSpeed * Time.deltaTime);

        m_playerAnimation.AnimateMovement(horizontal, vertical, m_sprint);
    }

}
