using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour {

    public static PlayerManagement Instance;
    private PhotonView m_photonView;

    private List<PlayerStats> PlayerStats = new List<PlayerStats>();

    private void Awake() {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }
    
    public void AddPlayerStats(PhotonPlayer photonPlayer) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer); //make sure the player is not already in the list
        
        if (index == -1) {
            PlayerStats.Add(new PlayerStats(photonPlayer, 30)); //initial health = 30
        }
    }

    public void ModifyHealth(PhotonPlayer photonPlayer, int value) {
        //Find the player in playerstats that we're going to modify
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);

        if (index != -1) {
            PlayerStats playerStats = PlayerStats[index];
            playerStats.Health += value;
            PlayerNetwork.Instance.NewHealth(photonPlayer, playerStats.Health);
        }
    }
}

//Hold data about each player - anticheating
public class PlayerStats {

    public PlayerStats(PhotonPlayer photonPlayer, int health) {
        PhotonPlayer = photonPlayer;
        Health = health;
    }

    public readonly PhotonPlayer PhotonPlayer; //Whenever you create this class you will assign the photonplayer and that will not change bc is readonly
    public int Health;
}
