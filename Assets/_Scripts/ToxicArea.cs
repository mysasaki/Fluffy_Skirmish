using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicArea : MonoBehaviour {

    /*private void OnTriggerEnter(Collider other) { //make the server handle the Ontrigger, not the clients
        //Caso nao queira que o host armazene todos os dados, comenta aqui. POrque caso o host saia da partida, todos os dados sobre vida vao se perder
        //Anti hacking. Apenas o master vai armazenar os dados dos outros clients, assim os clients nao podem manipular os proprios dados
        //if (!PhotonNetwork.isMasterClient) 
        //    return;

        //get the photonview component from object that hit
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (photonView != null && photonView.isMine) {
            //PlayerManagement.Instance.ModifyHealth(photonView.owner, -10); //anti hacking
            Player player = other.GetComponent<Player>();
            player.Health -= 10; //jeito izi de fazer. E nao da problema caso o host saia da partida
        }
    }*/

}
