﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Player m_owner;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            Destroy(gameObject);

            PlayerManagement.Instance.DealDamage(m_owner.ID, player.ID, 20);
        }
    }
}
