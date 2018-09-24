using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraRig : MonoBehaviour {

    public Transform m_target;
    public bool m_autoTargetPlayer; //auto target 
    public LayerMask m_wallLayers; //help detect walls

    public enum Shoulder {
        Right, Left
    }

    public Shoulder shoulder;

    [System.Serializable]
    public class CameraSettings {//have all the camera settings
        [Header("-Positioning -")]
        public Vector3 CamPositionOffsetLeft;
        public Vector3 CamPositionOffsetRight;

        [Header("-Camera Options-")]
        public float MouseXSensitivity = 3.0f;
        public float MouseYSensitivity = 2.0f;
        public float MinAngle = -30.0f; //min angle we point up and down
        public float MaxAngle = 70.0f;
        public float RotationSpeed = 5.0f; //lerp speed
        public float MaxCheckDist = 0.1f; //Distance we check the wall for

        [Header("-Zoom-")]
        public float FieldOfView = 70.0f;
        public float ZoomFieldOfView = 30.0f;
        public float ZoomSpeed = 3.0f; //lerp for zoom

        [Header("-Visual Options-")]
        public float HideMeshWhenDistance = 0.5f; //when were too close to camera, it will hide the mesh from the camera
        
    }

    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings { //Hold all the strings that going to call input and return values
        public string VerticalAxis = "Mouse X";
        public string HorizontalAxis = "Mouse Y";
        public string AimButton = "Fire2";
        public string SwitchShoulderButton = "Fire4";

    }

    [SerializeField]
    public InputSettings input;

    [System.Serializable]
    public class MovementSettings {
        public float movementLerpSpeed = 5.0f;
    }

    [SerializeField]
    public MovementSettings movement;

    private Transform m_pivot;
    private Camera m_mainCamera;
    private float m_newX = 0.0f; //values that are going to be passed when we move the mouse
    private float m_newY = 0.0f;

    private void Start() {
        m_mainCamera = Camera.main;
        m_pivot = transform.GetChild(0);
    }

    private void Update() {
        if (m_target == null)
            return;

        RotateCamera();
        CheckWall(); //check for wall behind it
        CheckMeshRenderer();
        Zoom(Input.GetButton(input.AimButton));

        if(Input.GetButtonDown(input.SwitchShoulderButton)) 
            SwitchShoulders();
            
        
    }

    //All functionality that follows player
    private void FixedUpdate() {
        if (m_target == null) //TargetPlayer() -> do tutorial mas talvez n precise por ser mo
            return;

        Vector3 targetPos = m_target.position;
        Quaternion targetRot = m_target.rotation;

        FollowTarget(targetPos, targetRot);
    }

    #region Camera
    //Rotates the camera with Input
    private void RotateCamera() {
        if (!m_pivot)
            return;

        m_newX += cameraSettings.MouseXSensitivity * Input.GetAxis(input.VerticalAxis);
        m_newY -= cameraSettings.MouseYSensitivity * Input.GetAxis(input.HorizontalAxis);

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = m_newY;
        eulerAngleAxis.y = m_newX;

        m_newX = Mathf.Repeat(m_newX, 360); //repeat when it hits 360. prevents bouncing
        m_newY = Mathf.Clamp(m_newY, cameraSettings.MinAngle, cameraSettings.MaxAngle);

        Quaternion newRotation = Quaternion.Slerp(m_pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * cameraSettings.RotationSpeed);
        m_pivot.localRotation = newRotation;
    }

    //Check wall and move camera up if it hits
    private void CheckWall() {
        if (!m_pivot || !m_mainCamera)
            return;

        RaycastHit hit;
        Transform mainCamTransform = m_mainCamera.transform;
        Vector3 mainCamPosition = mainCamTransform.position;
        Vector3 pivotPosition = m_pivot.position;

        Vector3 start = pivotPosition;
        Vector3 direction = mainCamPosition - pivotPosition;

        float distance = Mathf.Abs(shoulder == Shoulder.Left ? cameraSettings.CamPositionOffsetLeft.z : cameraSettings.CamPositionOffsetRight.z);

        if (Physics.SphereCast(start, cameraSettings.MaxCheckDist, direction, out hit, distance, m_wallLayers)) {
            //MoveCamUp(hit, pivotPosition, direction, mainCamTransform);
        } else {

            switch(shoulder) { //positioning the camera based on the shoulder
                case Shoulder.Left:
                    PositionCamera(cameraSettings.CamPositionOffsetLeft);    
                    break;

                case Shoulder.Right:
                    PositionCamera(cameraSettings.CamPositionOffsetRight);
                    break;
            }
        }
    }

    //Moves the camera forward when we hit a wall
    private void MoveCamUp(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform cameraT) {
        float hitDist = hit.distance;
        Vector3 sphereCastCenter = pivotPos + (dir.normalized * hitDist); //center of the sphere cast so we can move the camera to the middle

        cameraT.position = sphereCastCenter; //move the camera to the center of the spherecast
        //Now the camera wont hit walls unless the object is not in the layer mask
    }

    //Positions camera localposition to given location
    private void PositionCamera(Vector3 cameraPos) {
        if (!m_mainCamera)
            return;

        Transform mainCamT = m_mainCamera.transform;
        Vector3 mainCamPos = mainCamT.localPosition;
        Vector3 newPos = Vector3.Lerp(mainCamPos, cameraPos, Time.deltaTime * movement.movementLerpSpeed);
        mainCamT.localPosition = newPos;
    }

    //Hide the meshes when too close to target
    private void CheckMeshRenderer() {
        if (!m_mainCamera || !m_target)
            return;

        SkinnedMeshRenderer[] meshes = m_target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform mainCamT = m_mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 targetPos = m_target.position;
        float dist = Vector3.Distance(mainCamPos, targetPos + m_target.up);

        if (meshes.Length > 0) {
            for (int i = 0; i < meshes.Length; i++) {
                if (dist <= cameraSettings.HideMeshWhenDistance)
                    meshes[i].enabled = false;
                else
                    meshes[i].enabled = true;
            }
        }
    }

    //Zoom camera in and out
    private void Zoom(bool isZooming) {
        if (!m_mainCamera)
            return;

        if(isZooming) {
            float newFieldOfView = Mathf.Lerp(m_mainCamera.fieldOfView, cameraSettings.ZoomFieldOfView, Time.deltaTime * cameraSettings.ZoomSpeed);
            m_mainCamera.fieldOfView = newFieldOfView;

        } else {
            float originalFieldOfView = Mathf.Lerp(m_mainCamera.fieldOfView, cameraSettings.FieldOfView, Time.deltaTime * cameraSettings.ZoomSpeed);
            m_mainCamera.fieldOfView = originalFieldOfView;
        }
    }

    //Switches the cameras shoulderview
    public void SwitchShoulders() {
        switch(shoulder) {
            case Shoulder.Left:
                shoulder = Shoulder.Right;                
                break;

            case Shoulder.Right:
                shoulder = Shoulder.Left;
                break;
        }
    }
    #endregion

    #region FollowPlayer
    //Finds the player gameobject and set it as target
    private void TargetsPlayer() {
        if (m_autoTargetPlayer) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if(player) {
                Transform playerT = player.transform;
                m_target = playerT;
            }         
        }
    }

    //Following the target with time.deltatime smoothly
    private void FollowTarget(Vector3 targetPos, Quaternion targetRot) {
        Vector3 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * movement.movementLerpSpeed);
        transform.position = newPos;
        
    }

    #endregion
}
