using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Player m_owner;
    public ParticleSystem myParticle;

    private void Start() {
        //myParticle = GetComponentInChildren<ParticleSystem>();
        StartCoroutine(DestroyOnTime());
    }

    private IEnumerator DestroyOnTime() {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        
        //Destroy(gameObject); boing 
        //Instancia decal ou sangue
        if (m_owner.ID != PhotonNetwork.player.ID) //apenas o dono da bala vai processar o dano pra que nao ocorra multiplicacao do dano por chamar o rpc multiplas vezes
            return;

        if (other.CompareTag("Player")) {
            Vector3 partPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.3f);
            Instantiate(myParticle, partPosition, transform.rotation);
            Destroy(gameObject);
            Player player = other.GetComponent<Player>();
            if (!player.IsDead && !player.Respawning) {
                print("ISDEAD " + player.IsDead);
                print("RESPAWNING " + player.Respawning);
                PlayerManagement.Instance.DealDamage(m_owner.ID, player.ID, 20, other.transform);
            }
        }
    }
}
