using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeover : MonoBehaviour {

    public LayerMask layerMask;

    private GameObject m_hitObject;
    private PhotonView m_photonView;
    private Player m_player;

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();

        if (m_photonView.isMine)
            m_player = GetComponent<Player>();
    }

    public int GetPlayerID() {
        return m_player.ID;
    }

    public void PickupWeapon() {
        if (!m_photonView.isMine)
            return;

        Vector3 origin = transform.position + transform.up;

        Collider[] objs;
        objs = Physics.OverlapSphere(origin, 3.0f, layerMask);

        foreach (Collider c in objs) {
            if (c.CompareTag("Weapon")) {
                m_hitObject = c.gameObject;
                WeaponTakeover weaponTakeover = m_hitObject.GetComponent<WeaponTakeover>();
                if (weaponTakeover.m_hasOwner) {
                    print("weapon has owner");
                    return;

                } else {
                    print("weapons has no owner");
                    m_photonView.RPC("RPC_WeaponTakeover", PhotonTargets.All, PhotonNetwork.player.ID, weaponTakeover.m_photonView.instantiationId);
                }
            }
        }
    }

    public void DropWeapon() {
        if (!m_photonView.isMine)
            return;

        PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();

        if (playerWeapon.currentWeapon != null) {
            m_photonView.RPC("RPC_WeaponDrop", PhotonTargets.All, PhotonNetwork.player.ID);
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
                if (weaponTakeover.m_photonView.instantiationId == weaponID) {
                    print("selected weapon " + w);
                    selectedWeapon = w;
                }
            }

            if (players.Length > 0) {
                foreach (GameObject p in players) {
                    PlayerTakeover playerTakeover = p.GetComponent<PlayerTakeover>();
                    

                    if (playerTakeover.GetPlayerID() == playerID) {
                        WeaponTakeover weaponTakeover = selectedWeapon.GetComponent<WeaponTakeover>();
                        weaponTakeover.TakeoverWeapon();
                        PlayerWeapon playerWeapon = p.GetComponent<PlayerWeapon>();
                        playerWeapon.PickupWeapon(selectedWeapon);
                        playerWeapon.PickupWeapon(selectedWeapon);
                        
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
                if (playerTakeover.GetPlayerID() == playerID) {
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
