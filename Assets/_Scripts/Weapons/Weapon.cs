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
        public float bulletSpread = 5.0f;
        public float fireRate = 3.0f;
        public LayerMask bulletLayers; //layers that bullet will hit
        public float range = 200.0f;

        [Header("-Effects-")]
        public GameObject decal;
        public GameObject clip;

        [Header("-Other-")]
        public float reloadDuration = 2.0f;
        public GameObject crosshairPrefab;

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
        public int carryingAmmo;
        public int clipAmmo;
        public int maxClipAmmo;
    }
    [SerializeField]
    public Ammunition ammo;

    public Ray shootRay { protected get; set; }
    public bool m_ownerAiming { get; set; }
    private WeaponHandler owner;
    private bool m_equipped;
    private bool m_pullingTrigger;
    private bool m_resettingCartridge;


    private void Start() {
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();

        if (weaponSettings.crosshairPrefab != null) {
            weaponSettings.crosshairPrefab = Instantiate(weaponSettings.crosshairPrefab);
            ToggleCrosshair(false);
        }
    }  

    private void Update() {
        if(owner) {
            DisableEnableComponents(false);

            if (m_equipped) {
                if(owner.userSettings.rightHand) {
                    Equip();

                    if(m_pullingTrigger) 
                        Fire(shootRay);

                    if(m_ownerAiming) {
                        PositionCrosshair(shootRay);

                    } else {
                        ToggleCrosshair(false);
                    }
                }
            } else {
                Unequip(weaponType);
            }
        } else {
            DisableEnableComponents(true);
            transform.SetParent(null);
            m_ownerAiming = false;
        }
    }

    //Fires the weapon
    private void Fire(Ray ray) {
        print("Pew pew");
        if (ammo.clipAmmo <= 0 || m_resettingCartridge || !weaponSettings.bulletSpawn)
            return;

        RaycastHit hit;
        Transform bulletSpawn = weaponSettings.bulletSpawn;
        Vector3 bulletSpawnPosition = bulletSpawn.position;
        Vector3 direction = ray.GetPoint(weaponSettings.range); //Mira em direçao ao centro da camera, utilizando o ray criado da camera

        direction += (Vector3)Random.insideUnitCircle * weaponSettings.bulletSpread;

        if (Physics.Raycast(bulletSpawnPosition, direction, out hit, weaponSettings.range, weaponSettings.bulletLayers)) {
            HitEffects(hit); //decal
        }

        ammo.clipAmmo--;
        m_resettingCartridge = true;
        StartCoroutine(LoadNextBullet());
    }

    private void HitEffects(RaycastHit hit) {
        if (hit.collider.gameObject.isStatic) {
            if (weaponSettings.decal) {
                Vector3 hitpoint = hit.point;
                Quaternion lookRotation = Quaternion.LookRotation(hit.normal);
                GameObject decal = Instantiate(weaponSettings.decal, hitpoint, lookRotation) as GameObject;

                Transform decalT = decal.transform;
                Transform hitT = hit.transform;
                decalT.SetParent(hitT);
                Destroy(decal, Random.Range(30.0f, 45.0f));
            }
        }
    }

    //Position crosshair to the point that we are aiming at
    private void PositionCrosshair(Ray ray) {
        RaycastHit hit;
        Transform bulletSpawn = weaponSettings.bulletSpawn;
        Vector3 bulletSpawnPoints = bulletSpawn.position;
        Vector3 direction = ray.GetPoint(weaponSettings.range); //Mira em direçao ao centro da camera, utilizando o ray criado da camera

        if (Physics.Raycast(bulletSpawnPoints, direction, out hit, weaponSettings.range, weaponSettings.bulletLayers)) {
            if(weaponSettings.crosshairPrefab != null) {
                ToggleCrosshair(true);
                weaponSettings.crosshairPrefab.transform.position = hit.point;
                weaponSettings.crosshairPrefab.transform.LookAt(Camera.main.transform);
            }
        } else {
            ToggleCrosshair(false);
        }
    }

    //Toggle on and off the crosshair prefab
    private void ToggleCrosshair(bool enabled) {
        if (weaponSettings.crosshairPrefab != null) {
            weaponSettings.crosshairPrefab.SetActive(enabled);
        }
    }

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

    //Equip this weapon to the hand
    private void Equip() {
        if (!owner)
            return;

        else if (!owner.userSettings.rightHand)
            return;

        transform.SetParent(owner.userSettings.rightHand);
        transform.localPosition = weaponSettings.equipPosition;
        Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
        transform.localRotation = equipRot;
    }

    //Unequips the weapon and places it to the desired location
    private void Unequip(WeaponType weaponType) {
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
    }

    //Loads the clip and calculates ammo
    public void LoadClip() {
        int ammoNeeded = ammo.maxClipAmmo - ammo.clipAmmo;
        
        if (ammoNeeded >= ammo.carryingAmmo) {
            ammo.clipAmmo = ammo.carryingAmmo;
            ammo.carryingAmmo = 0;
        } else {
            ammo.carryingAmmo -= ammoNeeded;
            ammo.clipAmmo = ammo.maxClipAmmo;
        }
    }

    //Sets the weapons equip state
    public void SetEquipped(bool equip) {
        m_equipped = equip;
    }

    //Pull the trigger
    public void PullTrigger(bool isPulling) {
        if (isPulling)
            print("Pull trigger");
        m_pullingTrigger = isPulling;
    }

    //Sets owner of the weapon
    public void SetOwner(WeaponHandler wp) {
        owner = wp;
    }
}
