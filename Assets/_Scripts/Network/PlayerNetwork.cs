using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour {
    public static PlayerNetwork Instance;
    private PhotonView m_photonView;
    private int m_playersInGame = 0;
    public Player m_currentPlayer;

    private void Awake() {
        Instance = this; //singleton
        m_photonView = GetComponent<PhotonView>();

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
        print("Master client loaded " + PhotonNetwork.player);
        m_photonView.RPC("RPC_LoadedGameScene", PhotonTargets.All, PhotonNetwork.player); //needs to call becase if the other client never joins, this rpc will never be called
        //tell all the other players that they should load scene
        m_photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others); //rpc: broadcast messsage to others
    }

    private void NonMasterLoadedGame() {
        print("client loaded. playerid " + PhotonNetwork.player);
        
        m_photonView.RPC("RPC_LoadedGameScene", PhotonTargets.All, PhotonNetwork.player);
    }

    #region RPC 
    [PunRPC]
    private void RPC_LoadGameOthers() {
        PhotonNetwork.LoadLevel("Game"); 
    }

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer) { //called on the master to tell how many players on the game   
        print("Player added: " + PhotonNetwork.player.NickName + " [" + PhotonNetwork.player.ID + "]");
        PlayerManagement.Instance.AddPlayer(PhotonNetwork.player.ID, PhotonNetwork.player.NickName, 100, 24);
        m_playersInGame++;
        if (m_playersInGame == PhotonNetwork.playerList.Length && PhotonNetwork.isMasterClient) { //all the players are in the game
            print("All players are in game scene");
            m_photonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
            PlayerManagement.Instance.AddPlayer(photonPlayer.ID, photonPlayer.NickName, 100, 24); //doublecheck
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer() {
        
        float randomZ = Random.Range(0f, 150f);
        float randomX = Random.Range(0f, 150f);

        GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), new Vector3(randomX, 5, randomZ), Quaternion.identity, 0);
        m_currentPlayer = obj.GetComponent<Player>();

        Player player = obj.GetComponent<Player>();
        player.ID = PhotonNetwork.player.ID;
        player.Name = PhotonNetwork.player.NickName;
        print("Created player with id: " + PhotonNetwork.player.ID + ", " + PhotonNetwork.player.NickName);
        //PlayerManagement.Instance.AssignInstantiatedPlayer(obj.GetInstanceID());
        //PlayerManagement.Instance.AddPlayer(photonView.instantiationId); //populate the playerStats in Playermanagement

    }
}
#endregion