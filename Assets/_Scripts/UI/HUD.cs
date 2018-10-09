using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Text ammoText;
    public Slider healthBar;
    public Text healthText;

    public void UpdateUI(PlayerWeapon playerWeapon, Player player) {
        if (playerWeapon) {

            if (playerWeapon.currentWeapon == null) {
                this.ammoText.text = "Unarmed";

            } else {
                this.ammoText.text = playerWeapon.currentWeapon.ammo.clipAmmo + "/" + player.Ammo;
            }
        }

        if (this.healthBar && this.healthText) {
            this.healthBar.value = Mathf.Round(player.Health);
            this.healthText.text = Mathf.Round(this.healthBar.value).ToString();
        }
    }
}
