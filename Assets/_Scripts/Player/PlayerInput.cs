﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private PlayerMovement m_playerMovement;
    private PhotonView m_photonView;
    private WeaponHandler m_weaponHandler;

    [System.Serializable]
    public class InputSettings {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string reloadButton = "Reload ";
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string dropWeaponButton = "DropWeapon";
        public string switchWeaponButton = "SwitchWeapon";
    }

    [SerializeField]
    private InputSettings input;

    [System.Serializable]
    public class OtherSettings {
        public float lookSpeed = 7.5f;
        public float lookDistance = 10.0f;
        public bool requireInputForTurn = false;
        public LayerMask aimDetectionLayers;
    }
    [SerializeField]
    public OtherSettings otherSettings;

    public bool m_debugAim;
    public Transform m_spine;
    private bool m_aiming;

    Camera m_mainCamera;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        if (!m_photonView.isMine)
            return;

        m_playerMovement = GetComponent<PlayerMovement>();
        m_mainCamera = Camera.main;
        m_weaponHandler = GetComponent<WeaponHandler>();
    }

    private void FixedUpdate() {
        if (!m_photonView.isMine)
            return;

        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
    }

    //Handles character Logic
    private void CharacterLogic() {
        if (!m_playerMovement)
            return;

        m_playerMovement.Move(Input.GetAxis(input.verticalAxis), Input.GetAxis(input.horizontalAxis));
        //m_playerAnimation.Animate(blabla)

    }

    //Handle camera logic
    private void CameraLookLogic() {
        if (!m_mainCamera)
            return;

        if (otherSettings.requireInputForTurn) {
            if (Input.GetAxis(input.horizontalAxis) != 0 || Input.GetAxis(input.verticalAxis) != 0)
                PlayerLook();

        } else {
            PlayerLook();
        }
    }

    //Handles all weapon logic
    private void WeaponLogic() {
        if (!m_weaponHandler)
            return;

        m_aiming = Input.GetButton(input.aimButton);
        if(m_weaponHandler.currentWeapon) {
            m_weaponHandler.Aim(m_aiming);
            otherSettings.requireInputForTurn = !m_aiming;
            m_weaponHandler.FingerOnTrigger(Input.GetButton(input.fireButton));

            if (Input.GetButtonDown(input.reloadButton))
                m_weaponHandler.Reload();

            if (Input.GetButtonDown(input.dropWeaponButton))
                m_weaponHandler.DropCurrentWeapon();

            if (Input.GetButtonDown(input.switchWeaponButton))
                m_weaponHandler.SwitchWeapons();
        }
    }

    //Maker player look at a forward point from the camera
    private void PlayerLook() {
        Transform mainCameraT = m_mainCamera.transform;
        Transform pivotT = mainCameraT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * otherSettings.lookDistance);

        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * otherSettings.lookSpeed);
        transform.rotation = newRotation;
    }
}
