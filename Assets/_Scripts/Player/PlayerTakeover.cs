using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeover : MonoBehaviour {

    public bool pickupInRange = false;
    public bool dropWeapon = false;
    public LayerMask layerMask;

    private GameObject m_hitObject;
    private PhotonView m_photonView;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
        
    }

    private void LateUpdate() {
        if (!m_photonView.isMine)
            return;

        Vector3 origin = transform.position + transform.up;
        
        if (pickupInRange) {
            print("1");
            Collider[] objs;
            objs = Physics.OverlapSphere(origin, 3.0f, layerMask);

            foreach (Collider c in objs) {
                print("2");
                if (c.CompareTag("Weapon")) {
                    m_hitObject = c.gameObject;
                    WeaponTakeover weaponTakeover = m_hitObject.GetComponent<WeaponTakeover>();
                    print("3");
                    if (weaponTakeover.m_hasOwner) {
                        print("weapon has owner");
                        return;

                    }  else {
                        print("weapons has no owner");
                        m_photonView.RPC("RPC_WeaponTakeover", PhotonTargets.All, this.m_photonView.instantiationId, weaponTakeover.m_photonView.instantiationId);
                    }
                }
            }
        }  

        if(dropWeapon) {
            PlayerWeapon weaponHandler = GetComponent<PlayerWeapon>();

            if (weaponHandler.currentWeapon != null) {
                m_photonView.RPC("RPC_WeaponDrop", PhotonTargets.All, this.m_photonView.instantiationId);
            }
        }
    }

    [PunRPC]
    private void RPC_WeaponTakeover(int playerID, int weaponID) {
        print("RpC weapon takeover");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        GameObject selectedWeapon = weapons[0];

        if(weapons.Length > 0 ) {
            foreach (GameObject w in weapons) {
                WeaponTakeover weaponTakeover = w.GetComponent<WeaponTakeover>();
                if (weaponTakeover.m_photonView.instantiationId == weaponID)
                    selectedWeapon = w;
            }

            if (players.Length > 0) {
                foreach (GameObject p in players) {
                    PlayerTakeover playerTakeover = p.GetComponent<PlayerTakeover>();

                    if (playerTakeover.m_photonView.instantiationId == playerID) {
                        WeaponTakeover weaponTakeover = selectedWeapon.GetComponent<WeaponTakeover>();
                        weaponTakeover.TakeoverWeapon();
                        PlayerWeapon weaponHandler = p.GetComponent<PlayerWeapon>();
                        weaponHandler.PickupWeapon(selectedWeapon);
                        
                         return;
                    }

                }

            }
        }    
    }

    [PunRPC]
    private void RPC_WeaponDrop(int playerID) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0) {
            foreach (GameObject p in players) {
                PlayerTakeover playerTakeover = p.GetComponent<PlayerTakeover>();
                if (playerTakeover.m_photonView.instantiationId == playerID) {
                    print("RPC DROPPED");
                    PlayerWeapon weaponHandler = p.GetComponent<PlayerWeapon>();
                    weaponHandler.DropCurrentWeapon();

                    return;
                }

            }

        }

    }

    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.up, 3.0f);
    //}

}
