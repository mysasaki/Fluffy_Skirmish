using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int ID;
    public string Name;
    public int Health = 100;
    public int Ammo = 24;
    public int Kill = 0;
    public int Death = 0;

    public void UpdateHealth(int health) {
        this.Health = health;

        if (Health <= 0) {
            //ded
        }
    }

    public void UpdateAmmo(int ammo) {
        this.Ammo = ammo;
    }
}
