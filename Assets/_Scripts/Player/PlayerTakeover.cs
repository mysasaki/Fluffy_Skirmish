using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeover : MonoBehaviour {

    public bool pickupInRange = false;
    public LayerMask layerMask;

    private GameObject m_hitObject;
    private PhotonView m_photonView;
    private WeaponHandler m_weaponHandler;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        
    }

    private void LateUpdate() {
        if (!m_photonView.isMine)
            return;

        Vector3 origin = transform.position + transform.up;
        
        if (pickupInRange) {
            Collider[] objs;
            objs = Physics.OverlapSphere(origin, 3.0f, layerMask);

            foreach (Collider c in objs) {
                if (c.CompareTag("Weapon")) {
                    m_hitObject = c.gameObject;
                    WeaponTakeover weaponTakeover = m_hitObject.GetComponent<WeaponTakeover>();

                    if (weaponTakeover.m_hasOwner)
                        return;

                    else {
                        m_photonView.RPC("RPC_WeaponTakeover", PhotonTargets.All, this.m_photonView.instantiationId, weaponTakeover.m_photonView.instantiationId);
                    }
                }
            }
        }  
    }

    [PunRPC]
    private void RPC_WeaponTakeover(int playerID, int weaponID) {
        print("RPC called " + playerID + ", " + weaponID);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        GameObject selectedWeapon = weapons[0];

        if(weapons.Length > 0 ) {
            foreach (GameObject w in weapons) {
                WeaponTakeover weaponTakeover = w.GetComponent<WeaponTakeover>();
                print("w " + weaponTakeover.m_photonView.instantiationId);
                if (weaponTakeover.m_photonView.instantiationId == weaponID)
                    selectedWeapon = w;
            }

            if (players.Length > 0) {
                foreach (GameObject p in players) {
                    PlayerTakeover playerTakeover = p.GetComponent<PlayerTakeover>();
                    print("p " + playerTakeover.m_photonView.instantiationId);
                    if (playerTakeover.m_photonView.instantiationId == playerID) {
                        WeaponTakeover weaponTakeover = selectedWeapon.GetComponent<WeaponTakeover>();
                        weaponTakeover.TakeoverWeapon();
                        m_weaponHandler = p.GetComponent<WeaponHandler>();
                        m_weaponHandler.PickupWeapon(selectedWeapon);
                        
                        return;
                    }

                }

            }
        }    
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up, 3.0f);
    }

}
