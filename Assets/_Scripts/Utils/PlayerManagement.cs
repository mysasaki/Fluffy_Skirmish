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

    //public void UpdatePlayerList() {
    //    print("Update List");
    //    DebugPrint();
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //    foreach (GameObject p in players) {
    //        PhotonView photonView = p.GetPhotonView();
    //        Player player = p.GetComponent<Player>();
    //        AddPlayer(photonView.instantiationId, player.Health, player.Ammo);

    //    }
    //}
    
    public void AddPlayer(int id, string name, int health, int ammo) {
        m_photonView.RPC("RPC_NewPlayer", PhotonTargets.All, id, name, health, ammo);
    }

    public void ModifyHealth(int id, int value) {
        m_photonView.RPC("RPC_NewHealth", PhotonTargets.All, id, value);

    }

    public void ModifyAmmo(int id, int value) {
        m_photonView.RPC("RPC_NewAmmo", PhotonTargets.All, id, value);
    }

    [PunRPC]
    private void RPC_NewPlayer(int id, string name, int health, int ammo) {
        print("RPC NEW PLAYER");
        int index = m_playerStatsList.FindIndex(x => x.ID == id); //make sure the player is not already in the list

        if (index == -1) {
            m_playerStatsList.Add(new PlayerStats(id, name, health, ammo)); //initial health = 30

        }
    }

    [PunRPC]
    private void RPC_NewHealth(int id, int health) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id); //Find the player in playerstats that we're going to modify


        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Health += health;

        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id) {
                
                player.UpdateHealth(m_playerStatsList[index].Health);
                return;
            }
        }

    }

    [PunRPC]
    private void RPC_NewAmmo(int id, int ammo) {
        print("RPC NEW AMMO");
        int index = m_playerStatsList.FindIndex(x => x.ID == id);

        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.Ammo += ammo;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id) {            
                player.UpdateAmmo(m_playerStatsList[index].Ammo);
                DebugPrint();
                return;
            }
        }
    }

    private void DebugPrint() {
        print("PlayerList Length: " + m_playerStatsList.Count);
        foreach (PlayerStats p in m_playerStatsList) {
            print(p.ID + " : " + p.Ammo);
        }
    }
}

//Hold data about each player - anticheating
public class PlayerStats {

    public PlayerStats(int id, string name, int health, int ammo) {
        ID = id;
        Name = name;
        Health = health;
        Ammo = ammo;
        Kills = 0;
        Death = 0;
    }

    public readonly int ID; //Whenever you create this class you will assign the photonplayer and that will not change bc is readonly
    public readonly string Name;
    public int Health;
    public int Ammo;
    public int Kills;
    public int Death;
}
