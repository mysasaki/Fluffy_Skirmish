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
        m_weaponHandler = GetComponent<WeaponHandler>();
    }

    private void LateUpdate() {
        if (!m_photonView.isMine)
            return;

        Vector3 origin = transform.position + transform.up;
        
        if (pickupInRange) {
            Collider[] objs;
            objs = Physics.OverlapSphere(origin, 3.0f, layerMask);

            foreach (Collider c in objs) {
                print("COLLIDER " + c);
                if (c.CompareTag("Weapon")) {
                    print("WEAPON ");
                    m_hitObject = c.gameObject;
                    WeaponTakeover weapon = m_hitObject.GetComponent<WeaponTakeover>();

                    if (weapon.m_hasOwner)
                        return;

                    else {
                        print("TAKEOVER 1");
                        weapon.TakeoverWeapon();
                        m_weaponHandler.PickupWeapon(m_hitObject);
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
