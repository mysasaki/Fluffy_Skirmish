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
        Ray r = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 origin = r.GetPoint(0);
        Vector3 point = r.GetPoint(50);
        Debug.DrawLine(origin, point);

        m_photonView.RPC("RPC_InstantiateBullet", PhotonTargets.All, origin, point);
    }

    [PunRPC]
    private void RPC_InstantiateBullet(Vector3 position, Vector3 target) {
       
        GameObject bulletGameObject = Instantiate(bulletPrefab);
        bulletGameObject.transform.position = position;
        bulletGameObject.transform.LookAt(target);
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.m_owner = GetComponent<Player>();
        Rigidbody bulleRb = bulletGameObject.GetComponent<Rigidbody>();
        bulleRb.AddForce(bulletGameObject.transform.forward * 200, ForceMode.Impulse);
    }
}
