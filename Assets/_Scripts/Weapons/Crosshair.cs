using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {


    public void PositionCrosshair(Ray ray, Weapon currentWeapon) {

        RaycastHit hit;
        Transform bulletSpawn = currentWeapon.weaponSettings.bulletSpawn;
        Vector3 bulletSpawnPoint = bulletSpawn.position;
        Vector3 direction = ray.GetPoint(currentWeapon.weaponSettings.range) - bulletSpawnPoint;

        if (Physics.Raycast(bulletSpawnPoint, direction, out hit, currentWeapon.weaponSettings.range, currentWeapon.weaponSettings.bulletLayers)) {

            ToggleCrosshair(true);
            gameObject.transform.position = hit.point;
            gameObject.transform.LookAt(Camera.main.transform);
        }
    }

    public void ToggleCrosshair(bool enabled) {
        gameObject.SetActive(enabled);
    }

    private void DeleteCrosshair() {
        Destroy(gameObject);
    }
}
