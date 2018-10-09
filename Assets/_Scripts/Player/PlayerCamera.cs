using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private PhotonView m_photonView;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        if (!m_photonView.isMine)
            return;

        GameObject cameraRig = GameObject.FindGameObjectWithTag("CameraRig");
        if (cameraRig != null) {
            CameraRig cameraFollow = cameraRig.GetComponent<CameraRig>();
            if (cameraFollow != null) {
                cameraFollow.m_target = gameObject.transform;
            }
        }
    }

}
