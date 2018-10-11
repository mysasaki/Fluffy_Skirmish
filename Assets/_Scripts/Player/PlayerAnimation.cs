using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : Photon.MonoBehaviour {

    private PhotonView m_photonView;
    private Animator animator;

    [System.Serializable]
    public class AnimationSettings {
        public string verticalParamater = "Vertical";
        public string horizontalParamater = "Horizontal";
        public string walkingParamater = "IsWalking";
        public string sprintingParamater = "IsSprinting";
        public string aimingParamater = "IsAiming";
        public string aimAngleParamater = "AimAngle";
    }
    [SerializeField]
    private AnimationSettings animationSettings;

    [System.Serializable]
    public class AnimationParameters {
        public float verticalMovement;
        public float horizontalMovement;
        public float aimAngle;
        public bool isWalking;
        public bool isSprinting;
        public bool isAiming;
    }
    [SerializeField]
    public AnimationParameters animationParameters;

    private void Start() {
        m_photonView = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();

    }

    private void FixedUpdate() {
        if (!m_photonView || !animator)
            return;

        animator.SetFloat(animationSettings.verticalParamater, animationParameters.verticalMovement);
        animator.SetFloat(animationSettings.horizontalParamater, animationParameters.horizontalMovement);
        animator.SetFloat(animationSettings.aimAngleParamater, animationParameters.aimAngle);
        animator.SetBool(animationSettings.walkingParamater, animationParameters.isWalking);
        animator.SetBool(animationSettings.sprintingParamater, animationParameters.isSprinting);
        animator.SetBool(animationSettings.aimingParamater, animationParameters.isAiming);

    }

    public void AnimateMovement(float horizontal, float vertical, bool isSprinting) {
        animationParameters.horizontalMovement = horizontal;
        animationParameters.verticalMovement = vertical;

        if(!isSprinting)
            animationParameters.isWalking = (!(horizontal == 0 && vertical == 0));
        else
            animationParameters.isSprinting = (!(horizontal == 0 && vertical == 0));

    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //check if we're sending the data
        if (stream.isWriting == true) {
            stream.SendNext(animationParameters.verticalMovement);
            stream.SendNext(animationParameters.horizontalMovement);      
            stream.SendNext(animationParameters.aimAngle);      
            stream.SendNext(animationParameters.isWalking);      
            stream.SendNext(animationParameters.isSprinting);      
            stream.SendNext(animationParameters.isAiming);      

        } else {
            animationParameters.verticalMovement = (float)stream.ReceiveNext(); //pulled first entry of stream
            animationParameters.horizontalMovement = (float)stream.ReceiveNext(); 
            animationParameters.aimAngle = (float)stream.ReceiveNext(); 
            animationParameters.isWalking = (bool)stream.ReceiveNext(); 
            animationParameters.isSprinting = (bool)stream.ReceiveNext(); 
            animationParameters.isAiming = (bool)stream.ReceiveNext(); 
            
        }
    }
}
