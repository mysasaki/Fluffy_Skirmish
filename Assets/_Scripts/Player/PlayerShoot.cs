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

    public void InstantiateBullet(Transform bulletSpawn) {
        if (!m_photonView.isMine)
            return;

        Vector3 position = bulletSpawn.position;
        Camera camera = Camera.main;
        Quaternion rotation = Quaternion.LookRotation(bulletSpawn.forward);
        m_photonView.RPC("RPC_InstantiateBullet", PhotonTargets.All, position, rotation);
    }

    [PunRPC]
    private void RPC_InstantiateBullet(Vector3 position, Quaternion rotation) {
       
        GameObject bulletGameObject = Instantiate(bulletPrefab, position, rotation);
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.m_owner = GetComponent<Player>();
        Rigidbody bulleRb = bulletGameObject.GetComponent<Rigidbody>();
        bulleRb.AddForce(bulletGameObject.transform.forward * 100, ForceMode.Impulse);
    }
}
