﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : MonoBehaviour {

    public int ID;
    public GameObject mesh;

    private PhotonView m_photonView;

    private void Start() {
        m_photonView = GetComponentInParent<PhotonView>();
        ID = m_photonView.viewID;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            PlayerManagement.Instance.ModifyAmmo(player.ID, 12);
            PickupManager.Instance.RespawnPickupAmmo(this.ID);

        } else if(other.CompareTag("Lava")) {
            PickupManager.Instance.RespawnPickupAmmo(this.ID);
        }
    }

    public void ToggleMesh(bool active) {
        mesh.SetActive(active);
    }

    public void RespawnAmmo(float posX, float posZ) {

        GameObject go = transform.parent.gameObject;
        go.transform.position = new Vector3(posX, 25, posZ);
        ToggleMesh(true);
    }
}
