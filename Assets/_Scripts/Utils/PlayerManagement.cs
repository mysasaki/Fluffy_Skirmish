using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour {

    public static PlayerManagement Instance;
    private PhotonView m_photonView;

    public List<PlayerStats> m_playerStatsList = new List<PlayerStats>();

    private void Awake() {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }
    
    public void AddPlayerStats(int photonPlayerID) {
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayerID == photonPlayerID); //make sure the player is not already in the list
        
        if (index == -1) {
            m_playerStatsList.Add(new PlayerStats(photonPlayerID, 100, 24)); //initial health = 30
        }
    }

    public void ModifyHealth(int photonPlayerID, int value) {
        //Find the player in playerstats that we're going to modify
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayerID == photonPlayerID);

        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Health += value;
            PlayerNetwork.Instance.NewHealth(photonPlayerID, playerStats.Health);
        }
    }

    public void ModifyAmmo(int photonPlayerID, int value) {
        int index = m_playerStatsList.FindIndex(x => x.PhotonPlayerID == photonPlayerID);

        if(index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Ammo += value;
            PlayerNetwork.Instance.NewAmmo(photonPlayerID, playerStats.Ammo);
        }
    }
}

//Hold data about each player - anticheating
public class PlayerStats {

    public PlayerStats(int photonPlayerID, int health, int ammo) {
        PhotonPlayerID = photonPlayerID;
        Health = health;
        Ammo = ammo;
    }

    public readonly int PhotonPlayerID; //Whenever you create this class you will assign the photonplayer and that will not change bc is readonly
    public int Health;
    public int Ammo;
}
