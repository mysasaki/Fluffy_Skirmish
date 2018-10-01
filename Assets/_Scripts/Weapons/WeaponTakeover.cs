using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTakeover : Photon.PunBehaviour {

    public bool m_hasOwner = false;
    public PhotonView m_photonView;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();

    }

    public void TakeoverWeapon() {
        if (m_hasOwner)
            return;

        m_photonView.RequestOwnership();
        m_hasOwner = true;
    }
  
}
