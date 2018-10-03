using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour {

    public static PlayerManagement Instance;
    private PhotonView m_photonView;

    private List<PlayerStats> m_playerStatsList = new List<PlayerStats>();

    private void Awake() {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }
    
    public void AddPlayerStats(PhotonPlayer photonPlayer) {
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayer == photonPlayer); //make sure the player is not already in the list
        
        if (index == -1) {
            m_playerStatsList.Add(new PlayerStats(photonPlayer, 100, 24)); //initial health = 30
        }
    }

    public void ModifyHealth(PhotonPlayer photonPlayer, int value) {
        //Find the player in playerstats that we're going to modify
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayer == photonPlayer);

        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Health += value;
            PlayerNetwork.Instance.NewHealth(photonPlayer, playerStats.Health);
        }
    }

    public void ModifyAmmo(PhotonPlayer photonPlayer, int value) {
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayer == photonPlayer);

        if(index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Ammo += value;
            PlayerNetwork.Instance.NewAmmo(photonPlayer, playerStats.Ammo);
        }
    }
}

//Hold data about each player - anticheating
public class PlayerStats {

    public PlayerStats(PhotonPlayer photonPlayer, int health, int ammo) {
        PhotonPlayer = photonPlayer;
        Health = health;
        Ammo = ammo;
    }

    public readonly PhotonPlayer PhotonPlayer; //Whenever you create this class you will assign the photonplayer and that will not change bc is readonly
    public int Health;
    public int Ammo;
}
