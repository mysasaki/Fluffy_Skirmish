using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {
    //TODO: Colocar animator aqui
	[System.Serializable]
    public class UserSettings {
        public Transform RightHand;
        public Transform PistolUnequipSpot;
        //public Transform RifleUnequipSpot;
    }

    [SerializeField]
    public UserSettings userSettings;

    public Weapon currentWeapon;
    public List<Weapon> weaponsList = new List<Weapon>();
    public int maxWeapons = 2;
    private bool m_aim;
    private bool m_reload;
    private int m_weaponType;
    private bool m_settingWeapon; //prevent to change weapons rapo=idly

    private void Start() {
        
    }

    private void Update() {
        
    }

    //Animates the character
    private void Animates() {

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

    public void Reload() {
        if (m_reload || !currentWeapon)
            return;

        if (currentWeapon.ammo.CarryingAmmo <= 0 || currentWeapon.ammo.ClipAmmo == currentWeapon.ammo.MaxClipAmmo)
            return;

        m_reload = true;
        StartCoroutine(StartReload());
    }

    private IEnumerator StartReload() {
        yield return new WaitForSeconds(currentWeapon.weaponSettings.ReloadDuration);
        currentWeapon.LoadClip();
        m_reload = false;
    }
}
