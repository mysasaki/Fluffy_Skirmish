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
        public Transform LeftHandIKTarget;
        public Vector3 SpineRotation;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class WeaponSettings {
        [Header("-Bullet Options")]
        public Transform BulletSpawn;
        public float Damage = 5.0f;
        public float BulletSpread = 5.0f;
        public float FireRate = 3.0f;
        public LayerMask BulletLayers; //layers that bullet will hit
        public float Range = 200.0f;

        [Header("-Effects-")]
        public GameObject MuzzleFlash;
        public GameObject Decal;
        public GameObject Shell;

        [Header("-Options-")]
        public float ReloadDuration = 2.0f;
        public Transform ShellEjectSpot;
        public float ShellEjectSpeed = 7.5f;

        [Header("-Positioning-")]
        public Vector3 EquipPosition;
        public Vector3 EquipRotation;
        public Vector3 UnequipPosition;
        public Vector3 UnequipRotation;

        //Animation if necessary
    }
    [SerializeField]
    public WeaponSettings weaponSettings;

    [System.Serializable]
    public class Ammunition {
        public int CarryingAmmo;
        public int ClipAmmo;
        public int MaxClipAmmo;
    }
    [SerializeField]
    public Ammunition ammo;

    private WeaponHandler owner;
    private bool m_equipped;
    private bool m_pullingTrigger;
    private bool m_resettingCartridge;


    private void Start() {
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();

    }

    private void Update() {
        if(owner) {
            DisableEnableComponents(false);

            if (m_equipped) {
                if(owner.userSettings.RightHand) {
                    Equip();

                    if(m_pullingTrigger) 
                        Fire();
                }
            } else {
                Unequip(weaponType);
            }
        } else {
            DisableEnableComponents(true);
            transform.SetParent(null);
        }
    }

    //Fires the weapon
    private void Fire() {
        if (ammo.ClipAmmo == 0 || m_resettingCartridge || !weaponSettings.BulletSpawn)
            return;

        RaycastHit hit;
        Transform bulletSpawn = weaponSettings.BulletSpawn;
        Vector3 bulletSpawnPoints = bulletSpawn.position;
        Vector3 direction = bulletSpawn.forward;

        direction += (Vector3)Random.insideUnitCircle * weaponSettings.BulletSpread;

        if (Physics.Raycast(bulletSpawnPoints, direction, out hit, weaponSettings.Range, weaponSettings.BulletLayers)) {
            #region Decal
            if(hit.collider.gameObject.isStatic) {
                if(weaponSettings.Decal) {
                    Vector3 hitpoint = hit.point;
                    Quaternion lookRotation = Quaternion.LookRotation(hit.normal);
                    GameObject decal = Instantiate(weaponSettings.Decal, hitpoint, lookRotation) as GameObject;

                    Transform decalT = decal.transform;
                    Transform hitT = hit.transform;
                    decalT.SetParent(hitT);
                    Destroy(decalT, Random.Range(30.0f, 45.0f));
                }
            }
            #endregion
        }

        #region Muzzle Flash
        if (weaponSettings.MuzzleFlash) {
            Vector3 bulletSpawnPos = weaponSettings.BulletSpawn.position;
            GameObject muzzleFlash = Instantiate(weaponSettings.MuzzleFlash, bulletSpawnPos, Quaternion.identity) as GameObject;
            Transform muzzleT = muzzleFlash.transform;
            muzzleT.SetParent(weaponSettings.BulletSpawn);
            Destroy(muzzleFlash, 1.0f);
        }
        #endregion

        #region Shell
        if (weaponSettings.Shell) {
            if(weaponSettings.ShellEjectSpot) {
                Vector3 shellEjectPos = weaponSettings.ShellEjectSpot.position;
                Quaternion shellEjectRot = weaponSettings.ShellEjectSpot.rotation;
                GameObject shell = Instantiate(weaponSettings.Shell, shellEjectPos, shellEjectRot) as GameObject;

                if (shell.GetComponent<Rigidbody>()) {
                    Rigidbody rb = shell.GetComponent<Rigidbody>();
                    rb.AddForce(weaponSettings.ShellEjectSpot.forward * weaponSettings.ShellEjectSpeed, ForceMode.Impulse);
                }
                Destroy(shell, Random.Range(30.0f, 45.0f));
            }
        }
        #endregion

        ammo.ClipAmmo--;
        m_resettingCartridge = true;
        StartCoroutine(LoadNextBullet());
    }

    //Loads the next bullet in chamber
    private IEnumerator LoadNextBullet() {
        yield return new WaitForSeconds(weaponSettings.FireRate);
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

        else if (!owner.userSettings.RightHand)
            return;

        transform.SetParent(owner.userSettings.RightHand);
        transform.localPosition = weaponSettings.EquipPosition;

        Quaternion equipRotation = Quaternion.Euler(weaponSettings.EquipRotation);
        transform.localRotation = equipRotation;
    }

    //Unequips the weapon and places it to the desired location
    private void Unequip(WeaponType weaponType) {
        if (!owner)
            return;

        switch (weaponType) {
            case WeaponType.Pistol:
                transform.SetParent(owner.userSettings.PistolUnequipSpot);
                break;

            default:
                print("Error in unequip weapon");
                break;
        }
        transform.localPosition = weaponSettings.UnequipPosition;
        Quaternion unequipRot = Quaternion.Euler(weaponSettings.UnequipRotation);
        transform.localRotation = unequipRot;
    }

    //Loads the clip and calculates ammo
    public void LoadClip() {
        int ammoNeeded = ammo.MaxClipAmmo - ammo.ClipAmmo;
        
        if (ammoNeeded >= ammo.CarryingAmmo) {
            ammo.ClipAmmo = ammo.CarryingAmmo;
            ammo.CarryingAmmo = 0;
        } else {
            ammo.CarryingAmmo -= ammoNeeded;
            ammo.ClipAmmo = ammo.MaxClipAmmo;
        }
    }

    //Sets the weapons equip state
    public void SetEquipped(bool equip) {
        m_equipped = equip;
    }

    //Pull the trigger
    public void PullTrigger(bool isPulling) {
        m_pullingTrigger = isPulling;
    }

    //Sets owner of the weapon
    public void SetOwner(WeaponHandler wp) {
        owner = wp;
    }
}
