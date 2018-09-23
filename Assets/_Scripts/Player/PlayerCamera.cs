using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private PhotonView PhotonView;

    private void Awake() {
        PhotonView = GetComponent<PhotonView>();
    }

    private void Start() {
        print("JOINED ROOM");
        if (!PhotonView.isMine)
            return;

        GameObject camera = Camera.main.gameObject;
        if (camera != null) {
            CameraFollow cameraFollow = camera.GetComponent<CameraFollow>();
            if (cameraFollow != null) {
                cameraFollow.Target = gameObject.transform;
            }
        }
    }

}
