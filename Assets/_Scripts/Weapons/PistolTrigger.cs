using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolTrigger : MonoBehaviour {

    public int ID;
    public GameObject mesh;
    private PhotonView m_photonView;

    private void Start() {
        m_photonView = GetComponentInParent<PhotonView>();
        ID = m_photonView.viewID;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Lava")) {
            print("respawnando pistol " + ID);
            PickupManager.Instance.RespawnPistol(this.ID);
        }
    }

    public void ToggleMesh(bool active) {
        mesh.SetActive(active);
    }

    public void RespawnPistol(float posX, float posZ) {
        GameObject go = transform.parent.gameObject;
        //WeaponTakeover wt = GetComponentInParent<WeaponTakeover>();
        //wt.m_hasOwner = false;
        go.transform.position = new Vector3(posX, 25, posZ);
        ToggleMesh(true);
    }
}
