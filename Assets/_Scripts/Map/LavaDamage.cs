using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (!PhotonNetwork.isMasterClient) //apenas o master vai processar o dano pra que nao ocorra multiplicacao do dano por chamar o rpc multiplas vezes
            return;

        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (!player.IsDead && !player.Respawning)
                PlayerManagement.Instance.InstantDeath(player.ID);
        }
    }
}
