 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {
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

        Animate();
    }

    //Animates the character
    private void Animate() {
        if (!currentWeapon)
            return;

        switch(currentWeapon.weaponType) {
            case Weapon.WeaponType.Pistol:
                m_weaponType = 1;
                break;

            //outros case de arma
            default:
                print("Error in weaponhandler");
                break;
        }
    }

    //Puts the finger on the trigger and asks if we pulled
    public void FingerOnTrigger(bool pulling) {
        if (!currentWeapon)
            return;
        if(pulling)
            print("Finger on trigger");
        currentWeapon.PullTrigger(pulling && m_aim && !m_reload);
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
