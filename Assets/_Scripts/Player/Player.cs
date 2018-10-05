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

    public bool IsDead = false;

    public void Update() {
        if (IsDead) {
            print("Player " + Name + " diededdened.");
            StartCoroutine(StartRespawnPlayer());
        }
    }

    private IEnumerator StartRespawnPlayer() {
        yield return new WaitForSeconds(10);
        RespawnPlayer();
        IsDead = false;
    }

    private void RespawnPlayer() {
        PlayerManagement.Instance.RespawnPlayer(this.ID);
    }

    public void UpdateHealth(int health) {
        this.Health = health;
    }

    public void UpdateAmmo(int ammo) {
        this.Ammo = ammo;
    }

    public void UpdateDeath(int death) {
        this.Death = death;       
    }

    public void UpdateKill(int kill) {
        this.Kill = kill;
    }
}
