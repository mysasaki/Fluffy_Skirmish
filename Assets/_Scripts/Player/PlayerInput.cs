using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private PlayerMovement m_playerMovement;
    private PlayerTakeover m_playerTakeover;

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
        public string pickupWeapon = "Pickup";
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

    public Camera m_tpsCamera;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        if (!m_photonView.isMine)
            return;

        m_playerMovement = GetComponent<PlayerMovement>();
        m_tpsCamera = Camera.main;
        m_weaponHandler = GetComponent<WeaponHandler>();
        m_playerTakeover = GetComponent<PlayerTakeover>();
    }

    private void FixedUpdate() {
        if (!m_photonView.isMine)
            return;

        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
    }

    private void LateUpdate() {
        if (m_weaponHandler) {
            if (m_weaponHandler.currentWeapon) {
                if (m_aiming)
                    PositionSpine();
            }
        }
    }

    //Position spine when aiming 
    private void PositionSpine() {
        if (!m_spine || !m_weaponHandler.currentWeapon || !m_tpsCamera)
            return;

        Transform mainCameraT = m_tpsCamera.transform;
        Vector3 mainCamPos = mainCameraT.position;
        Vector3 dir = mainCameraT.forward;
        Ray ray = new Ray(mainCamPos, dir);
        m_spine.LookAt(ray.GetPoint(50));

        Vector3 eulerAngleOffset = m_weaponHandler.currentWeapon.userSettings.spineRotation;
        m_spine.Rotate(eulerAngleOffset);
        print("EULER ANGLE: " + eulerAngleOffset);

    }

    //Handles character Logic
    private void CharacterLogic() {
        if (!m_playerMovement)
            return;

        m_playerMovement.Move(Input.GetAxis(input.verticalAxis), Input.GetAxis(input.horizontalAxis));

        if (!m_playerTakeover)
            return;

        if (Input.GetButton(input.pickupWeapon)) {
            m_playerTakeover.pickupInRange = true;
        }

        if (Input.GetButtonUp(input.pickupWeapon)) {
            m_playerTakeover.pickupInRange = false;
        }
    }

    //Handle camera logic
    private void CameraLookLogic() {
        if (!m_tpsCamera)
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

        m_aiming = Input.GetButton(input.aimButton) || m_debugAim;
        if (m_weaponHandler.currentWeapon) {
            m_weaponHandler.Aim(m_aiming);
            //otherSettings.requireInputForTurn = !m_aiming;
            m_weaponHandler.FingerOnTrigger(Input.GetButton(input.fireButton));

            if (Input.GetButton(input.reloadButton))
                m_weaponHandler.Reload();

            #region DropWeapon
            if (Input.GetButton(input.dropWeaponButton)) 
                m_playerTakeover.dropWeapon = true;
            if(Input.GetButtonUp(input.dropWeaponButton))
                m_playerTakeover.dropWeapon = false;
            #endregion

            if (!m_weaponHandler.currentWeapon)
                return;

            m_weaponHandler.currentWeapon.shootRay = new Ray(m_tpsCamera.transform.position, m_tpsCamera.transform.forward);
        }
    }

    //Maker player look at a forward point from the camera
    private void PlayerLook() {
        Transform mainCameraT = m_tpsCamera.transform;
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
