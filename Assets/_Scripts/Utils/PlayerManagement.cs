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

    public void AddPlayer(int id, string name, int health, int ammo) {
        m_photonView.RPC("RPC_NewPlayer", PhotonTargets.All, id, name, health, ammo);
    }

    //public void ModifyHealth(int id, int value) {
    //    m_photonView.RPC("RPC_NewHealth", PhotonTargets.All, id, value);

    //}

    public void DealDamage(int id_owner, int id_other, int value) {
        m_photonView.RPC("RPC_DealDamage", PhotonTargets.All, id_other, value);
        print("0");

        if (CheckDeath(id_other, value)) {
            m_photonView.RPC("RPC_PlayerDie", PhotonTargets.All, id_owner, id_other, value);
        }
    }

    public void ModifyAmmo(int id, int value) {
        m_photonView.RPC("RPC_NewAmmo", PhotonTargets.All, id, value);
    }

    private bool CheckDeath(int id_player, int value) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id_player);

        if (index == -1)
            return false;

        PlayerStats player = m_playerStatsList[index];
        int health = player.Health;

        if (health - value <= 0)
            return true;

        return false;

    }

    public void RespawnPlayer(int id_player) {
        m_photonView.RPC("RPC_RespawnPlayer", PhotonTargets.All, id_player);
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
    private void RPC_DealDamage(int id_other, int damage) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id_other);
        print("1");
        if (index != -1) {

            PlayerStats otherPlayer = m_playerStatsList[index];
            otherPlayer.Health -= damage;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_other) {
                player.UpdateHealth(m_playerStatsList[index].Health);
                return;
            }
        }
    }

    [PunRPC]
    private void RPC_PlayerDie(int id_owner, int id_other, int value) {
        int index_other = m_playerStatsList.FindIndex(x => x.ID == id_other);
        if (index_other != -1) {
            PlayerStats other = m_playerStatsList[index_other];
            other.Death += 1;
            other.IsDead = true;
        }

        int index_owner = m_playerStatsList.FindIndex(y => y.ID == id_owner);
        if (index_owner != -1) {
            PlayerStats owner = m_playerStatsList[index_owner];
            owner.Kills += 1;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_other)
                player.UpdateDeath(m_playerStatsList[index_other].Death);

            if (player.ID == id_owner)
                player.UpdateKill(m_playerStatsList[index_owner].Kills);
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

    [PunRPC]
    private void RPC_RespawnPlayer(int id) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id);
        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.IsDead = false;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();

            if (player.ID == id) {
                print("Respawning");
                float randomZ = Random.Range(0f, 150f);
                float randomX = Random.Range(0f, 150f);
                p.transform.position = new Vector3(randomX, 5, randomZ);
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
        IsDead = false;
    }

    public readonly int ID; //Whenever you create this class you will assign the photonplayer and that will not change bc is readonly
    public readonly string Name;
    public int Health;
    public int Ammo;
    public int Kills;
    public int Death;
    public bool IsDead;
}
