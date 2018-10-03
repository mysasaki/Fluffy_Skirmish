using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour {
    public static PlayerNetwork Instance;
    public string m_playerName { get; private set; } //pode dar get em qualquer script, porem set private
    private PhotonView m_photonView;
    private int m_playersInGame = 0;

    public Player m_currentPlayer;

    private void Awake() {
        Instance = this; //singleton
        m_photonView = GetComponent<PhotonView>();
        m_playerName = "Player#" + Random.Range(1000, 9999); //Dá um nome ao player pra quando acessar o lobby

        PhotonNetwork.sendRate = 60; ///defaul = 20
        PhotonNetwork.sendRateOnSerialize = 30; //default = 10. Pode ficar muito pesado acima dos valores default

        SceneManager.sceneLoaded += OnSceneFinishedLoading; //cria um delegate usando a scenemanager
    }

    //Methods to perfom when change scene occur
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) { //callback da unity 
        if (scene.name == "Game") {
            if (PhotonNetwork.isMasterClient) {
                MasterLoadedGame();
            } else {
                NonMasterLoadedGame();
            }
        } 
    }

    private void MasterLoadedGame() { //mastercliente deu load na scene
        m_photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player); //needs to call becase if the other client never joins, this rpc will never be called
        //tell all the other players that they should load scene
        m_photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others); //rpc: broadcast messsage to others
    }

    private void NonMasterLoadedGame() {
        m_photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    public void NewHealth(PhotonPlayer photonPlayer, int health) {
        m_photonView.RPC("RPC_NewHealth", photonPlayer, health);
    }

    public void NewAmmo(PhotonPlayer photonPlayer, int ammo) {
        m_photonView.RPC("RPC_NewAmmo", photonPlayer, ammo);
    }

    #region RPC 
    [PunRPC]
    private void RPC_LoadGameOthers() {
        PhotonNetwork.LoadLevel(1); 
    }

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer) { //called on the master to tell how many players on the game

        //populate the playerStats in Playermanagement
        PlayerManagement.Instance.AddPlayerStats(photonPlayer);

        m_playersInGame++;
        if (m_playersInGame == PhotonNetwork.playerList.Length) { //all the players are in the game
            print("All players are in game scene");
            m_photonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void RPC_NewHealth(int health) {
        if (m_currentPlayer == null)  //verify if there's an object to destroy
            return;

        if (health <= 0)
            PhotonNetwork.Destroy(m_currentPlayer.gameObject); //ded
        else
            m_currentPlayer.Health = health; //passa a vida pro player. Assim ele nao consegue manipular com hacks

    }

    [PunRPC]
    private void RPC_NewAmmo(int ammo) {
        if (m_currentPlayer == null)
            return;

        m_currentPlayer.Ammo = ammo;
    }

    [PunRPC]
    private void RPC_CreatePlayer() {
        float randomZ = Random.Range(0f, 150f);
        float randomX = Random.Range(0f, 150f);

        GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), new Vector3(randomX, 5, randomZ), Quaternion.identity, 0);
        m_currentPlayer = obj.GetComponent<Player>();
        
    }
}
#endregion