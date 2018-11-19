using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : MonoBehaviour {

    public int ID;
    public GameObject mesh;

    private PhotonView m_photonView;
    
    private void Start() {
        m_photonView = GetComponentInParent<PhotonView>();
        ID = m_photonView.viewID; //id que vai ser usado pro pickup manager futuramente pra controlar qual pickup ja foi coletado e pa
    }

    public void OnTriggerEnter(Collider other) {
        if (!PhotonNetwork.player.IsLocal)
            return;

        if (other.CompareTag("Player")) {
            print("get health");
            Player player = other.GetComponent<Player>();
            PlayerAudio pa = player.GetComponent<PlayerAudio>();
            pa.PickupAudio();
            PlayerManagement.Instance.RestoreHealth(player.ID, 10);
            PickupManager.Instance.RespawnPickupHealth(this.ID);

        } else if (other.CompareTag("Lava")) {
            PickupManager.Instance.RespawnPickupHealth(this.ID);
        }
    }

    public void ToggleMesh(bool active) {
        mesh.SetActive(active);
    }

    public void RespawnHealth(float posX, float posZ) {
        GameObject go = transform.parent.gameObject;
        go.transform.position = new Vector3(posX, 25, posZ);
        float rotY = Random.Range(0f, 360f);
        go.transform.Rotate(0, rotY, 0);
        ToggleMesh(true);
    }
}
