using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    // *** VARIABLES *** //
    [SerializeField]
    private int m_round;
    public int Round {
        get {
            return m_round;
        }
    }

    [SerializeField]
    private float timeToNextClosePB;
    public float updownTime = 30f;
    public static float timeToNextClose = 70f; //tempo para o primeiro fechamento de setor de uma partida

    private GameObject go_aux; //gameObject auxiliar para armazenar setor que irá fechar

    [SerializeField]
    private GameObject[] sectors; //array de setores

    [SerializeField]
    private List<GameObject> sectorToBeClosed; //lista de setores escolhidos na rodada

    [SerializeField]
    private List<GameObject> sectorsToBeOpened; //lista de setores que estavam fechados e irão abrir

    [SerializeField]
    private MapMenu mapMenu;

    //vectors usados para fazer o lerp de ida e volta
    Vector3 pos, newPos;
    Vector3 pos2, newPos2;

    private PhotonView m_photonView;
    private List<int> idsToBeClosed = new List<int>();
    private List<int> idsToBeOpened = new List<int>();

    private bool flagStart = false;

    // *** FUNCTIONS *** //
    void Start() {
        m_photonView = GetComponent<PhotonView>();

        if (!PhotonNetwork.isMasterClient) {
            GameSetting.Instance.Destroy();
            return;
        }

        SetGameMatchTime();
        GameSetting.Instance.Destroy();
        timeToNextClosePB = timeToNextClose;
        m_photonView.RPC("RPC_SetClientMatchTime", PhotonTargets.Others, timeToNextClose, updownTime);
        
        MatchTimeControl.OnMinimumPlayersReached += StartMapMovement;
    }

    public void SetGameMatchTime() {
        MatchTime mt = GameSetting.Instance.matchTime;

        switch(mt) {
            case MatchTime.Short:
                timeToNextClose = 30;
                updownTime = 10;
                break;

            case MatchTime.Medium:
                timeToNextClose = 50;
                updownTime = 20;
                break;

            case MatchTime.Long:
                timeToNextClose = 70;
                updownTime = 30;
                break;
        }
    }

    public void StartMapMovement() {
        //flagStart = true;
        StartCoroutine("Routine");
    }

    IEnumerator Routine() {
        /*for (m_round = 1; m_round < 10; m_round++)
        {
            sectorToBeClosed.Clear();
            ChooseRandomSector();
            yield return new WaitForSeconds(timeToNextClose);
            StartCoroutine("DisableSector");
            timeToNextClose = 10f;
        }*/

        //if (!PhotonNetwork.isMasterClient) //só o master client vai fazer o esquema de escolher terreno
        //    yield return null;

        m_round = 1;
        while (m_round < 10) {
            sectorToBeClosed.Clear();
            idsToBeClosed.Clear();
            ChooseRandomSector();
            yield return new WaitForSeconds(timeToNextClose);
            m_photonView.RPC("RPC_DisableSectors", PhotonTargets.All, idsToBeClosed.ToArray());

        }
    }

    IEnumerator MoveObject(Vector3 source, Vector3 target, float overTime, GameObject go_aux) {
        float startTime = Time.time;
        while (Time.time < startTime + overTime) {
            go_aux.transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        go_aux.transform.position = target;
    }

    void Update() {

        if (!flagStart && PhotonNetwork.isMasterClient) {
            VerifyIfAllPlayersInRoom();
        }

        if (m_round >= 9) //sai do update
            return;

        timeToNextClose -= Time.deltaTime;
        //showTimeToNextClose = timeToNextClose;
    }

    private void VerifyIfAllPlayersInRoom() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if(players.Length == PhotonNetwork.room.PlayerCount) {
            m_photonView.RPC("RPC_StartTime", PhotonTargets.All);
        }
    }

    void ChooseRandomSector() {
        int randomNumber;

        for (int i = 1; i <= m_round; i++) {
            randomNumber = Random.Range(0, 9);
            go_aux = sectors[randomNumber];

            if (CheckIfAlreadyChosen()) {
                while (CheckIfAlreadyChosen()) {
                    randomNumber = Random.Range(0, 9);
                    go_aux = sectors[randomNumber];
                    CheckIfAlreadyChosen();
                }
                sectorToBeClosed.Add(go_aux);
            }
            else
                sectorToBeClosed.Add(go_aux);
        }

        foreach (GameObject t in sectorToBeClosed) {
            TerrainID terrainId = t.GetComponent<TerrainID>();
            idsToBeClosed.Add(terrainId.id);
        }
        m_photonView.RPC("RPC_ChoseSectorsToClose", PhotonTargets.All, idsToBeClosed.ToArray());
    }

    bool CheckIfAlreadyChosen() {
        if (sectorToBeClosed.Contains(go_aux))
            return true;
        else
            return false;
    }

    IEnumerator DisableSector() {

        /*if(m_round != 1)
            yield return new WaitForSeconds(5f);*/

        sectorsToBeOpened.Clear();
        idsToBeOpened.Clear();

        foreach (GameObject go_aux in sectorToBeClosed) {
            pos = new Vector3(go_aux.transform.position.x, go_aux.transform.position.y, go_aux.transform.position.z);
            newPos = new Vector3(go_aux.transform.position.x, -100.0f, go_aux.transform.position.z);

            StartCoroutine(MoveObject(pos, newPos, updownTime, go_aux));

            sectorsToBeOpened.Add(go_aux);

            TerrainID t = go_aux.GetComponent<TerrainID>();
            idsToBeOpened.Add(t.id);
        }

        yield return new WaitForSeconds(updownTime);
        if (PhotonNetwork.isMasterClient)
            m_photonView.RPC("RPC_EnableSectors", PhotonTargets.All, idsToBeOpened.ToArray());
    }

    private void ActivateSector() {
        print("Activate sector " + sectorsToBeOpened.Count);
        if (m_round < 2)
            return;

        foreach (GameObject go_aux in sectorsToBeOpened) {
            pos2 = new Vector3(go_aux.transform.position.x, go_aux.transform.position.y, go_aux.transform.position.z);
            newPos2 = new Vector3(go_aux.transform.position.x, 0, go_aux.transform.position.z);

            StartCoroutine(MoveObject(pos2, newPos2, updownTime, go_aux));
        }
    }

    [PunRPC]
    private void RPC_SetClientMatchTime(float closeTime, float openTime) {
        this.timeToNextClosePB = closeTime;
        this.updownTime = openTime;
    }

    [PunRPC]
    private void RPC_StartTime() {
        flagStart = true;
        timeToNextClose = timeToNextClosePB;
    }

    [PunRPC]
    private void RPC_ChoseSectorsToClose(int[] ids) {

        List<int> aux = new List<int>();
        foreach (int i in ids) {
            aux.Add(i);
        }
        mapMenu.ToClose(aux);
        //foreach (GameObject s in sectors) {
        //    TerrainID terrainID = s.GetComponent<TerrainID>();

        //    if (ids.Contains(terrainID.id)) {
        //        idsToBeClosed.Add(terrainID.id);
        //    }
        //}

    }

    [PunRPC]
    private void RPC_DisableSectors(int[] ids) {
        List<int> aux = new List<int>();
        timeToNextClose = timeToNextClosePB;
        print("ROUND " + m_round);
        m_round++;

        print("RPC DISABLE SECTOR " + ids.Length);
        sectorToBeClosed.Clear();

        foreach (int i in ids) {
            aux.Add(i);
        }
        mapMenu.Close(aux);

        foreach (GameObject s in sectors) {
            TerrainID terrainID = s.GetComponent<TerrainID>();

            if(ids.Contains(terrainID.id)) {
                sectorToBeClosed.Add(s);
                StartCoroutine("DisableSector");
                aux.Add(terrainID.id);
            }
        }
    }

    [PunRPC]
    private void RPC_EnableSectors(int[] ids) {
        print("RPC ENABLE SECTOR " + ids.Length);
        sectorsToBeOpened.Clear();

        List<int> aux = new List<int>();
        foreach (int  i in ids) {
            aux.Add(i);
        }
        mapMenu.Open(aux);

        foreach (GameObject s in sectors) {
            TerrainID t = s.GetComponent<TerrainID>();

            if(ids.Contains(t.id)) {
                sectorsToBeOpened.Add(s);
            }
        }

        ActivateSector();
    }

    private void OnDisable() {
        MatchTimeControl.OnMinimumPlayersReached -= StartMapMovement;
        StopAllCoroutines();
    }
}

public enum MatchTime {
    Short, Medium, Long
}
