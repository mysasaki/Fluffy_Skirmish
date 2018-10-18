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
    public static float timeToNextClose; //tempo para o primeiro fechamento de setor de uma partida

    private GameObject go_aux; //gameObject auxiliar para armazenar setor que irá fechar

    [SerializeField]
    private GameObject[] sectors; //array de setores

    [SerializeField]
    private List<GameObject> sectorToBeClosed; //lista de setores escolhidos na rodada

    [SerializeField]
    private List<GameObject> sectorsToBeOpened; //lista de setores que estavam fechados e irão abrir

    //vectors usados para fazer o lerp de ida e volta
    Vector3 pos, newPos;
    Vector3 pos2, newPos2;


    // *** FUNCTIONS *** //
    void Start() {
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

        m_round = 1;
        while (m_round < 10) {
            sectorToBeClosed.Clear();
            ChooseRandomSector();
            yield return new WaitForSeconds(timeToNextClose);
            StartCoroutine("DisableSector");
            timeToNextClose = 20f;
            m_round++;
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
        if (m_round >= 9) //sai do update
            return;

        timeToNextClose -= Time.deltaTime;
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

        foreach (GameObject go_aux in sectorToBeClosed) {
            pos = new Vector3(go_aux.transform.position.x, go_aux.transform.position.y, go_aux.transform.position.z);
            newPos = new Vector3(go_aux.transform.position.x, -100, go_aux.transform.position.z);

            StartCoroutine(MoveObject(pos, newPos, 5f, go_aux));

            sectorsToBeOpened.Add(go_aux);
        }

        yield return new WaitForSeconds(5f);
        ActivateSector();
    }

    void ActivateSector() {
        if (m_round < 2)
            return;

        foreach (GameObject go_aux in sectorsToBeOpened) {
            pos2 = new Vector3(go_aux.transform.position.x, go_aux.transform.position.y, go_aux.transform.position.z);
            newPos2 = new Vector3(go_aux.transform.position.x, 0, go_aux.transform.position.z);

            StartCoroutine(MoveObject(pos2, newPos2, 5f, go_aux));
        }
    }
}
