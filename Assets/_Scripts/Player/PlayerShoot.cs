using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe responsavel por instanciar tiro, mandar pro network
public class PlayerShoot : MonoBehaviour {

    private PhotonView m_photonView;
    public float bulletSpeed = 300;
    public GameObject bulletPrefab;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    public void ShootPewPew(Vector3 position) {
        RaycastHit hit;
        Camera camera = Camera.main;
        print("pew pew");
        //TODO: som de tiro
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 200)) {

            if (hit.transform.tag == "Player") {
                print("hit me with your best shot");            
                Player player = hit.transform.GetComponent<Player>();
                Player owner = GetComponent<Player>();

                print("player " + player.ID);
                print("owner " + owner.ID);
                if (!player.IsDead && !player.Respawning) {
                    PlayerManagement.Instance.DealDamage(owner.ID, player.ID, 20, hit.transform);
                }
            } else {
                //TODO: alguma particula de poeira
            }
        }

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
        bulleRb.AddForce(bulletGameObject.transform.forward * bulletSpeed, ForceMode.Impulse);
    }
}
