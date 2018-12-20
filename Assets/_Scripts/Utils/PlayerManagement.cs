using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour {

    public static PlayerManagement Instance;

    private PhotonView m_photonView;
    private KillFeed m_killFeed;
    private ScoreLayoutGroup m_scoreLayoutGroup;
    private TerrainManager m_terrainManager;

    public Ragdoll ragdoll;

    public List<PlayerStats> m_playerStatsList = new List<PlayerStats>();

    private void Awake() {

        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        m_killFeed = FindObjectOfType<KillFeed>();
        m_scoreLayoutGroup = FindObjectOfType<ScoreLayoutGroup>();
        m_terrainManager = FindObjectOfType<TerrainManager>();

        DamageTextController.Initialize();
    }

    public void AddPlayer(int id, string name, int health, int ammo) {
        m_photonView.RPC("RPC_NewPlayer", PhotonTargets.All, id, name, health, ammo);
    }

    //public void ModifyHealth(int id, int value) {
    //    m_photonView.RPC("RPC_NewHealth", PhotonTargets.All, id, value);

    //}

    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        int index = m_playerStatsList.FindIndex(x => x.ID == otherPlayer.ID);

        if (index != -1)
            m_playerStatsList.RemoveAt(index);

    }

    public void DealDamage(int id_owner, int id_other, int value, Transform otherTransform) {
        print("del damange");
        DamageTextController.CreateDamageText(value.ToString(), otherTransform); //popup de texto
        m_photonView.RPC("RPC_DealDamage", PhotonTargets.All, id_owner, id_other, value);
    }

    public void RestoreHealth(int id_player, int value) {
        print("restore health");
        m_photonView.RPC("RPC_RestoreHealth", PhotonTargets.All, id_player, value);
    }

    public void InstantDeath(int id_player) {
        print("Instant death");
        m_photonView.RPC("RPC_InstantDeath", PhotonTargets.All, id_player);
    }

    public void ModifyAmmo(int id, int value) {
        m_photonView.RPC("RPC_NewAmmo", PhotonTargets.All, id, value);
    }

    private bool CheckDeath(int id_player) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id_player);

        if (index == -1)
            return false;

        PlayerStats player = m_playerStatsList[index];

        if (player.Health <= 0)
            return true;

        return false;
    }

    public void RespawnPlayer(int id_player) {
        if (PhotonNetwork.player.ID != id_player)
            return;

        Vector2 respawn = CalculateRespawnPosition();

        m_photonView.RPC("RPC_RespawnPlayer", PhotonTargets.All, id_player, respawn.x, respawn.y);
    }

    public Vector2 CalculateRespawnPosition() {
        if(!m_terrainManager)
            m_terrainManager = FindObjectOfType<TerrainManager>();

        List<int> sectors = new List<int>();
        for (int i = 1; i <= 9; i++) { //inicializa
            sectors.Add(i);
        }

        foreach (int id in m_terrainManager.idsToBeClosed) { //remove os terrenos onde nao da pra respawn
            sectors.Remove(id);
        }

        int n = Random.Range(0, sectors.Count); //sorteia numero do terreno
        TerrainID[] terrains = FindObjectsOfType<TerrainID>();

        TerrainID randomTerrain = terrains[n];
        float randomZ = Random.Range(randomTerrain.minRangeZ, randomTerrain.maxRangeZ);
        float randomX = Random.Range(randomTerrain.minRangeX, randomTerrain.maxRangeX);

        return new Vector2(randomX, randomZ);

    }

    [PunRPC]
    private void RPC_NewPlayer(int id, string name, int health, int ammo) {
        int index = m_playerStatsList.FindIndex(x => x.Name == name); //make sure the player is not already in the list
        if (index == -1) {
            m_playerStatsList.Add(new PlayerStats(id, name, health, ammo)); //initial health = 30

            //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //foreach (GameObject p in players) {

            //}
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
    private void RPC_DealDamage(int id_owner, int id_other, int damage) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id_other);

        if (index != -1) {
            PlayerStats otherPlayer = m_playerStatsList[index];
            otherPlayer.Health -= damage;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_other) {
                player.UpdateHealth(m_playerStatsList[index].Health);

                if (CheckDeath(id_other))
                    RPC_PlayerDie(id_owner, id_other);
                   // m_photonView.RPC("RPC_PlayerDie", PhotonTargets.Others, id_owner, id_other);
            }
        }
    }

    [PunRPC]
    private void RPC_RestoreHealth(int id_player, int value) {
        print("RPC Restore health " + value);
        int index = m_playerStatsList.FindIndex(x => x.ID == id_player);

        if(index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];

            if (playerStats.Health + value > 100)
                playerStats.Health = 100;
            else
                playerStats.Health += value;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if(player.ID == id_player) {
                player.UpdateHealth(m_playerStatsList[index].Health);
            }
        }
    }

    //[PunRPC]
    private void RPC_PlayerDie(int id_owner, int id_other) {

        if (!m_killFeed)
            m_killFeed = FindObjectOfType<KillFeed>();

        if (!m_scoreLayoutGroup)
            m_scoreLayoutGroup = FindObjectOfType<ScoreLayoutGroup>();

        int index_other = m_playerStatsList.FindIndex(x => x.ID == id_other); //atualiza o placar
        Kill kill = new Kill();
        if (index_other != -1) {
            PlayerStats other = m_playerStatsList[index_other];
            other.Death += 1;
            other.IsDead = true;
            kill.Victim = other.Name;
        }

        int index_owner = m_playerStatsList.FindIndex(y => y.ID == id_owner);
        if (index_owner != -1) {
            PlayerStats owner = m_playerStatsList[index_owner];
            owner.Kills += 1;
            kill.Killer = owner.Name;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_other) {
                player.UpdateDeath(m_playerStatsList[index_other].Death);
                player.IsDead = true;
            }

            if (player.ID == id_owner)
                player.UpdateKill(m_playerStatsList[index_owner].Kills);
        }

        m_killFeed.AddKill(kill, PhotonNetwork.player.ID);
        //m_scoreLayoutGroup.UpdateScoreboard();
    }

    [PunRPC]
    private void RPC_NewAmmo(int id, int ammo) {
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
                return;
            }
        }
    }

    [PunRPC]
    private void RPC_RespawnPlayer(int id, float newX, float newZ) {
        int index = m_playerStatsList.FindIndex(x => x.ID == id);
        if (index != -1) {
            PlayerStats playerStats = m_playerStatsList[index];
            playerStats.IsDead = false;
        }


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();

            if (player.ID == id) {
                p.transform.position = new Vector3(newX, 25, newZ);
                player.ToggleMesh(true);
                player.Health = 100;
                m_playerStatsList[index].Health = 100;
                player.FinishRespawn();

                return;
            }
        }


    }

    [PunRPC]
    private void RPC_InstantDeath(int id_player) {
        print("RPC instant death");
        //if (!m_killFeed)
        //    m_killFeed = FindObjectOfType<KillFeed>();

        ; ; Kill kill = new Kill();
        int index_player = m_playerStatsList.FindIndex(x => x.ID == id_player);
        if (index_player != -1) {
            PlayerStats other = m_playerStatsList[index_player];
            other.Death += 1;
            other.IsDead = true;
            //kill.Killer = other.Name;
            //kill.Victim = other.Name;
            //m_killFeed.AddKill(kill, PhotonNetwork.player.ID);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_player) {
                player.UpdateDeath(m_playerStatsList[index_player].Death);
                player.IsDead = true;
            }

        }
    }

    #region Debug

    public void DebugKill(int id_player) {
        m_photonView.RPC("RPC_DebugKill", PhotonTargets.All, id_player);
    }

    [PunRPC]
    private void RPC_DebugKill(int id_player) {

        if (!m_killFeed)
            m_killFeed = FindObjectOfType<KillFeed>();

        print("RPC player debug die");

        Kill kill = new Kill();
        int index_player = m_playerStatsList.FindIndex(x => x.ID == id_player);
        if (index_player != -1) {
            PlayerStats other = m_playerStatsList[index_player];
            other.Death += 1;
            other.IsDead = true;
            kill.Killer = other.Name;
            kill.Victim = other.Name;

            //print("KILL FEED " + m_killFeed);
            m_killFeed.AddKill(kill, PhotonNetwork.player.ID);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Player player = p.GetComponent<Player>();
            if (player.ID == id_player) {
                player.UpdateDeath(m_playerStatsList[index_player].Death);
                player.IsDead = true;
                
            }

        }
    }

    #endregion
    private void DebugPrint() {
        print("PlayerList Length: " + m_playerStatsList.Count);
        foreach (PlayerStats p in m_playerStatsList) {
            print("PLAYER ID " + p.ID);
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
