using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour {
    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; } //pode dar get em qualquer script, porem set private
    private PhotonView PhotonView;
    private int PlayersInGame = 0;

    public PlayerMovement CurrentPlayer;

    private void Awake() {
        Instance = this; //singleton
        PhotonView = GetComponent<PhotonView>();
        PlayerName = "Player#" + Random.Range(1000, 9999); //Dá um nome ao player pra quando acessar o lobby

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
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player); //needs to call becase if the other client never joins, this rpc will never be called
        //tell all the other players that they should load scene
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others); //rpc: broadcast messsage to others
    }

    private void NonMasterLoadedGame() {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    public void NewHealth(PhotonPlayer photonPlayer, int health) {
        PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
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

        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length) { //all the players are in the game
            print("All players are in game scene");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void RPC_NewHealth(int health) {
        if (CurrentPlayer == null)  //verify if there's an object to destroy
            return;

        if (health <= 0)
            PhotonNetwork.Destroy(CurrentPlayer.gameObject); //ded
        else
            CurrentPlayer.Health = health; //passa a vida pro player. Assim ele nao consegue manipular com hacks

    }

    [PunRPC]
    private void RPC_CreatePlayer() {
        float randomZ = Random.Range(0f, 150f);
        float randomX = Random.Range(0f, 150f);

        GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "NewPlayer"), new Vector3(randomX, 0, randomZ), Quaternion.identity, 0);
        CurrentPlayer = obj.GetComponent<PlayerMovement>();
        
    }
}
#endregion