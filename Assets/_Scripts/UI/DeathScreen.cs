using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour {
    public Animator animator;
    public GameObject deathScreenContainer;

    public void Start() {
        deathScreenContainer.SetActive(false);
    }

    public void ActivateDeathScreen() {
        deathScreenContainer.SetActive(true);
        StartCoroutine("DeactivateDeathScreen");
    }

    private IEnumerator DeactivateDeathScreen() {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        deathScreenContainer.SetActive(false);
    }
}
