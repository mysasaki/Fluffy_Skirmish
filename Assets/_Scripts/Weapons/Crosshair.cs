using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    
    public void PositionCrosshair(Transform bulletSpawn, Quaternion rotation) {

        transform.position = bulletSpawn.position + bulletSpawn.forward * 50;
        transform.rotation = bulletSpawn.rotation;

        transform.LookAt(Camera.main.transform);       
    }

    public void ToggleCrosshair(bool enabled) {
        gameObject.SetActive(enabled);
    }

    private void DeleteCrosshair() {
        Destroy(gameObject);
    }
}
