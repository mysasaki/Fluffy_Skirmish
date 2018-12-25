using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    //public void PositionCrosshair(Transform bulletSpawn, Quaternion rotation) {

    //    transform.position = bulletSpawn.position + bulletSpawn.forward * 50;
    //    transform.rotation = bulletSpawn.rotation;

    //    transform.LookAt(Camera.main.transform);       
    //}

    public void PlayRecoil() {
        print("play recoil");
        animator.SetBool("Recoil", true);
        StartCoroutine(WaitRecoil());
    }

    private IEnumerator WaitRecoil() {
        print("coroutine recoil");
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Recoil", false);
    }

    public void ToggleCrosshair(bool enabled) {
        gameObject.SetActive(enabled);
    }

    private void DeleteCrosshair() {
        Destroy(gameObject);
    }
}
