using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe responsavel por instanciar tiro, mandar pro network
public class PlayerShoot : MonoBehaviour {

    private PhotonView m_photonView;

    public GameObject bulletPrefab;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    public void InstantiateBullet(Vector3 position) {
        if (!m_photonView.isMine)
            return;

        Camera camera = Camera.main;
        Quaternion rotation = Quaternion.LookRotation(camera.transform.forward);
        m_photonView.RPC("RPC_InstantiateBullet", PhotonTargets.All, position, rotation);
    }

    [PunRPC]
    private void RPC_InstantiateBullet(Vector3 position, Quaternion rotation) {
       
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody bulleRb = bullet.GetComponent<Rigidbody>();
        bulleRb.AddForce(bullet.transform.forward * 50, ForceMode.Impulse);
    }


}
