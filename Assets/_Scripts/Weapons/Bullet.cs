﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Player m_owner;

    private void Start() {
        StartCoroutine(DestroyOnTime());
    }

    private IEnumerator DestroyOnTime() {
        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (m_owner.ID != PhotonNetwork.player.ID) //apenas o dono da bala vai processar o dano pra que nao ocorra multiplicacao do dano por chamar o rpc multiplas vezes
            return;

        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            Destroy(gameObject);

            PlayerManagement.Instance.DealDamage(m_owner.ID, player.ID, 20);
        }
    }
}
