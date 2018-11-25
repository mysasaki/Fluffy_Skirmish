using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour {

    private Collider collider;
    private Rigidbody rigidBody;
    
    public enum WeaponType {
        Pistol //baseball bat etc
    }
    public WeaponType weaponType;

    [System.Serializable]
    public class UserSettings { //settings that vary by the user
        public Transform leftHandIKTarget;
        public Vector3 spineRotation;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class WeaponSettings {
        [Header("-Bullet Options")]
        public Transform bulletSpawn;
        public float damage = 5.0f;
        public float fireRate = 3.0f;

        [Header("-Effects-")]
        public GameObject decal;
        public GameObject bullet;

        [Header("-Other-")]
        public float reloadDuration = 2.0f;

        [Header("-Positioning-")]
        public Vector3 equipPosition;
        public Vector3 equipRotation;
        public Vector3 unequipPosition;
        public Vector3 unequipRotation;

        //Animation if necessary
    }
    [SerializeField]
    public WeaponSettings weaponSettings;

    [System.Serializable]
    public class Ammunition {
        public int clipAmmo;
        public int maxClipAmmo;
    }
    [SerializeField]
    public Ammunition ammo;

    public Ray shootRay { protected get; set; }
    public bool m_ownerAiming { get; set; }
    public PlayerWeapon owner;
    private bool m_equipped;
    private bool m_resettingCartridge;
    private PlayerShoot m_playerShoot;
    public Player m_player;

    private WeaponAudio m_weaponAudio;

    private void Start() {
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        m_weaponAudio = GetComponent<WeaponAudio>();
    }  

    private void Update() {
        if(owner) {
            DisableEnableComponents(false);

            if (m_equipped) {
                if(owner.userSettings.rightHand) {
                    Equip();
                }

            } else {

                if (weaponSettings.bulletSpawn.childCount > 0) {
                    foreach (Transform t in weaponSettings.bulletSpawn.GetComponentsInChildren<Transform>()) {
                        if (t != weaponSettings.bulletSpawn)
                            Destroy(t.gameObject);
                    }
                }
                Unequip(weaponType);
            }
        } else {
            DisableEnableComponents(true);
            transform.SetParent(null);
            m_ownerAiming = false;
        }
    }

    //Fires the weapon
    public void Fire() {
        print("Pew pew");
        if (ammo.clipAmmo <= 0 || m_resettingCartridge || !weaponSettings.bulletSpawn || !m_equipped)
            return;

        Transform bulletSpawn = weaponSettings.bulletSpawn;
        m_playerShoot.InstantiateBullet(bulletSpawn.position);
        m_weaponAudio.FireAudio();

        ammo.clipAmmo--;
        m_resettingCartridge = true;
        StartCoroutine(LoadNextBullet());
    }

    //private void HitEffects(RaycastHit hit) {
    //    if (hit.collider.gameObject.isStatic) {
    //        if (weaponSettings.decal) {
    //            Vector3 hitpoint = hit.point;
    //            Quaternion lookRotation = Quaternion.LookRotation(hit.normal);
    //            GameObject decal = Instantiate(weaponSettings.decal, hitpoint, lookRotation) as GameObject;

    //            Transform decalT = decal.transform;
    //            Transform hitT = hit.transform;
    //            decalT.SetParent(hitT);
    //            Destroy(decal, Random.Range(30.0f, 45.0f));
    //        }
    //    }
    //}

    //Loads the next bullet in chamber
    private IEnumerator LoadNextBullet() {
        yield return new WaitForSeconds(weaponSettings.fireRate);
        m_resettingCartridge = false;
    }

    //Disables or enables collider and rb
    private void DisableEnableComponents(bool enabled) {
        if(!enabled) {
            rigidBody.isKinematic = true;
            collider.enabled = false;
        } else {
            rigidBody.isKinematic = false;
            collider.enabled = true;

        }
    }

    public void Reload() {
        m_weaponAudio.ReloadAudio();
    }

    //Equip this weapon to the hand
    private void Equip() {
        if (!owner)
            return;

        else if (!owner.userSettings.rightHand)
            return;

        transform.SetParent(owner.userSettings.rightHand);
        transform.localPosition = weaponSettings.equipPosition;
        m_playerShoot = owner.GetComponent<PlayerShoot>();
        m_player = owner.GetComponent<Player>();

        Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
        transform.localRotation = equipRot;
    }

    //Unequips the weapon and places it to the desired location
    private void Unequip(WeaponType weaponType) {
        print("UNEQUIP");
        if (!owner)
            return;

        switch (weaponType) {
            case WeaponType.Pistol:
                transform.SetParent(owner.userSettings.pistolUnequipSpot);
                break;

            default:
                print("Error in unequip weapon");
                break;
        }
        transform.localPosition = weaponSettings.unequipPosition;

        Quaternion unequipRot = Quaternion.Euler(weaponSettings.unequipRotation);
        transform.localRotation = unequipRot;
        m_playerShoot = null;
        m_player = null;
    }

    //Loads the clip and calculates ammo
    public void LoadClip() {
        int ammoNeeded = ammo.maxClipAmmo - ammo.clipAmmo;
        PhotonView photonView = owner.GetComponentInParent<PhotonView>();
        print("Load clip: " + photonView.owner.ID);

        if (ammoNeeded >= m_player.Ammo) {
            ammo.clipAmmo = m_player.Ammo;
            PlayerManagement.Instance.ModifyAmmo(photonView.owner.ID, -m_player.Ammo);

        } else {
            ammo.clipAmmo = ammo.maxClipAmmo;
            PlayerManagement.Instance.ModifyAmmo(photonView.owner.ID, -ammoNeeded);
            
        }
     
    }

    //Sets the weapons equip state
    public void SetEquipped(bool equip) {
        m_equipped = equip;
    }


    //Sets owner of the weapon
    public void SetOwner(PlayerWeapon wp) {
        owner = wp;

        if (wp == null)
            m_player = null;
    }

}
