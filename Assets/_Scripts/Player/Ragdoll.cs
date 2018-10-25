using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

    public Animator animator;
    [SerializeField]
    private Rigidbody[] bodyParts;
    public bool active;
    //private MoveController moveController;

    private void Start() {
        bodyParts = transform.GetComponentsInChildren<Rigidbody>();
        EnableRagdoll(false);
        //moveController = GetComponent<MoveController>();
    }

    /*public override void Die() {

        base.Die();
        EnableRagdoll(true);
        animator.enabled = false;
    }*/
    private void Update() {
        EnableRagdoll(active);
    }

    public void EnableRagdoll(bool value) {
        animator.enabled = !value;
        for (int i = 0; i < bodyParts.Length; i++) {
            bodyParts[i].isKinematic = !value;
        }
    }
}

