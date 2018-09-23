using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Photon.MonoBehaviour {

    private PhotonView PhotonView;
    private Vector3 TargetPosition;
    private Quaternion TargetRotation;
    public float Health;

    private void Awake() {
        PhotonView = GetComponent<PhotonView>();
    }

    private void Update() {
        if (PhotonView.isMine)
            CheckInput();
        else
            SmoothMove();
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
            TargetPosition = (Vector3) stream.ReceiveNext(); //pulled first entry of stream
            TargetRotation = (Quaternion)stream.ReceiveNext();
            //Health = (float)stream.ReceiveNext();
        }
    }

    private void SmoothMove() {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.25f); //the higher the value, more torwards the target the move is
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, 500 * Time.deltaTime);
    }

    private void CheckInput() { //Handle player movement
        float moveSpeed = 100;
        float rotateSpeed = 500f;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        transform.position += transform.forward * (vertical * moveSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, horizontal * rotateSpeed * Time.deltaTime, 0));
    }
}
