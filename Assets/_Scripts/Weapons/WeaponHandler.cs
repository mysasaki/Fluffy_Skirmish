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
    public List<Weapon> weaponsList = new List<Weapon>();
    public int maxWeapons = 2;
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
            AddWeaponToList(currentWeapon);

            if (currentWeapon.ammo.clipAmmo <= 0)
                Reload();

            if (weaponsList.Count > 0) {
                for (int i = 0; i < weaponsList.Count; i++) {
                    if(weaponsList[i] != currentWeapon) {
                        weaponsList[i].SetEquipped(false);
                        weaponsList[i].SetOwner(this);

                    }
                }
            }
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

    //Add weapon to the weaponsList
    private void AddWeaponToList(Weapon weapon) {
        if (weaponsList.Contains(weapon))
            return;

        weaponsList.Add(weapon);
    }

    //Puts the finger on the trigger and asks if we pulled
    public void FingerOnTrigger(bool pulling) {
        if (!currentWeapon)
            return;

        currentWeapon.PullTrigger(pulling);
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

        currentWeapon.SetEquipped(false);
        currentWeapon.SetOwner(null);
        weaponsList.Remove(currentWeapon);
        currentWeapon = null;
    }

    //Switches to the next weapon
    public void SwitchWeapons() {
        if (m_settingWeapon)
            return;

        if (currentWeapon) {
            int currentWeaponIndex = weaponsList.IndexOf(currentWeapon);
            int nextIndex = (currentWeaponIndex + 1) % weaponsList.Count;

            currentWeapon = weaponsList[nextIndex];
        } else {
            currentWeapon = weaponsList[0];
        }
        m_settingWeapon = true;
        StartCoroutine(StopSettingWeapon());
    }

    private IEnumerator StopSettingWeapon() {
        yield return new WaitForSeconds(0.7f);
        m_settingWeapon = false;
    }

    //private void OnAnimatorIK() {}
}
