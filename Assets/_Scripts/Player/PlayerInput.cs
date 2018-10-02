using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public PlayerWeapon m_playerWeapon { get; protected set; }
    public PlayerMovement m_playerMovement { get; protected set; }
    private PlayerTakeover m_playerTakeover;
    private PhotonView m_photonView;

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

    public Camera tpsCamera;
    public GameObject crosshairPrefab;
    private Crosshair m_crosshair;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        if (!m_photonView.isMine)
            return;

        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerWeapon = GetComponent<PlayerWeapon>();
        m_playerTakeover = GetComponent<PlayerTakeover>();

        tpsCamera = Camera.main;

        if (crosshairPrefab != null) {
            crosshairPrefab = Instantiate(crosshairPrefab);
            m_crosshair = crosshairPrefab.GetComponent<Crosshair>();
            m_crosshair.ToggleCrosshair(false);
        }
    }

    private void FixedUpdate() {
        if (!m_photonView.isMine)
            return;

        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
    }

    private void LateUpdate() {
        if (m_playerWeapon) {
            if (m_playerWeapon.currentWeapon) {
                if (m_aiming)
                    PositionSpine();
            }
        }
    }

    //Position spine when aiming 
    private void PositionSpine() {
        if (!m_spine || !m_playerWeapon.currentWeapon || !tpsCamera)
            return;

        Transform mainCameraT = tpsCamera.transform;
        Vector3 mainCamPos = mainCameraT.position;
        Vector3 dir = mainCameraT.forward;
        Ray ray = new Ray(mainCamPos, dir);
        m_spine.LookAt(ray.GetPoint(50));

        Vector3 eulerAngleOffset = m_playerWeapon.currentWeapon.userSettings.spineRotation;
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
        if (!tpsCamera)
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
        if (!m_playerWeapon)
            return;

        m_aiming = Input.GetButton(input.aimButton) || m_debugAim;
        m_playerWeapon.Aim(m_aiming);

        if (m_playerWeapon.currentWeapon) {
            Ray aimRay = new Ray(tpsCamera.transform.position, tpsCamera.transform.forward);

            if (Input.GetButton(input.fireButton) && m_aiming)
                m_playerWeapon.FireWeapon(aimRay);

            if (Input.GetButton(input.reloadButton))
                m_playerWeapon.Reload();

            #region DropWeapon
            if (Input.GetButton(input.dropWeaponButton)) 
                m_playerTakeover.dropWeapon = true;
            if(Input.GetButtonUp(input.dropWeaponButton))
                m_playerTakeover.dropWeapon = false;
            #endregion

            if(m_aiming) 
                m_crosshair.ToggleCrosshair(true);
                
             else 
                m_crosshair.ToggleCrosshair(false);
            

        } else {
            m_crosshair.ToggleCrosshair(false);
        }
    }

    //Maker player look at a forward point from the camera
    private void PlayerLook() {
        Transform mainCameraT = tpsCamera.transform;
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(tpsCamera.transform.position, tpsCamera.transform.forward * 10));
    }
}
