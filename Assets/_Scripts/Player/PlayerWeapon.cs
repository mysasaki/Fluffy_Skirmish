 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    //TODO: Colocar animator aqui
	[System.Serializable]
    public class UserSettings {
        public Transform rightHand;
        public Transform pistolUnequipSpot;
        //public Transform RifleUnequipSpot;
    }

    [SerializeField]
    public UserSettings userSettings;

    public PhotonView m_photonView;
    public Weapon currentWeapon;
    private bool m_aim;
    private bool m_reload;
    private int m_weaponType;
    private bool m_settingWeapon; //prevent to change weapons rapo=idly

    private void Awake() {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update() {
        if (!m_photonView.isMine)
            return;

        if(currentWeapon) {
            currentWeapon.SetEquipped(true);
            currentWeapon.SetOwner(this);
            currentWeapon.m_ownerAiming = m_aim;

            if (currentWeapon.ammo.clipAmmo <= 0)
                Reload();

            if (m_reload)
                if (m_settingWeapon)
                    m_reload = false;
        }
    }


    public void FireWeapon(Ray ray) {
        if(currentWeapon.ammo.clipAmmo == 0) {
            Reload();
            return;
        }

        currentWeapon.Fire(ray);
    }

    //Reloads the current weapon
    public void Reload() {
        if (m_reload || !currentWeapon)
            return;

        if (currentWeapon.ammo.carryingAmmo <= 0 || currentWeapon.ammo.clipAmmo == currentWeapon.ammo.maxClipAmmo)
            return;

        m_reload = true;
        StartCoroutine(StartReload());
    }

    //Stops the reloading of the weapon
    private IEnumerator StartReload() {
        yield return new WaitForSeconds(currentWeapon.weaponSettings.reloadDuration);
        currentWeapon.LoadClip();
        m_reload = false;
    }

    //Sets out aiming bool 
    public void Aim(bool aiming) {
        m_aim = aiming;
    }

    //Drops currentWeapon
    public void DropCurrentWeapon() {
        if (!currentWeapon)
            return;

        print("DROPPED WEAPON");

        WeaponTakeover weaponTakeover = currentWeapon.GetComponent<WeaponTakeover>();
        weaponTakeover.DropWeapon();
        currentWeapon.SetEquipped(false);
        currentWeapon.SetOwner(null);
        currentWeapon = null;
    }

    public void PickupWeapon(GameObject pickup) {

        if (currentWeapon)
            DropCurrentWeapon();

        Weapon weapon = pickup.GetComponent<Weapon>();
        if (!weapon)
            return;

        weapon.SetOwner(this);
        weapon.SetEquipped(true);

        Weapon pickupWeapon = pickup.GetComponent<Weapon>();
        currentWeapon = pickupWeapon;
    }

    //private void OnAnimatorIK() {}
}
