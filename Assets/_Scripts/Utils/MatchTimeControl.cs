using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MatchTimeControl : MonoBehaviour {

    //quantidade minima de players
    [SerializeField]
    private int m_minimumPlayers;
    public int minimumPlayers {
        get {
            return m_minimumPlayers;
        }
    }

    //Tempo total da partida.Pode ser customizavel pelo player ou não
    [SerializeField]
    private float m_matchTime;
    public float matchTime {
        get {
            return m_matchTime;
        }
    }

   //Controla o tempo atual da partida
    [SerializeField]
    private int m_currentTime;
    public int currentTime {
        get {
            return m_currentTime;
        }
    }

    //indica o fim da partida
    [SerializeField]
    private bool m_endMatch;
    public bool endMatch {
        get {
            return m_endMatch;
        }
    }

    //evento a ser disparado quando se atinge o numero minimo de players na sala
    //Faz com o que o tempo comece a contar
    public delegate void CountDownEvents();
    public static event CountDownEvents OnMinimumPlayersReached;
    //evento pra quando acabar o tempo
    public static event CountDownEvents OnTimeOver;
    [SerializeField]
    private bool eventTriggered;

    public string nextScene;
    public float waitToNextScene;

    public Text endText;

	// Use this for initialization
	void Start () {
        OnMinimumPlayersReached += CountDown;  //Assina o evento com a função de countdown
        OnTimeOver += HandleEndMatch; //assina o evento com a função de fim de tempo
        //m_matchTime = 5f; //define o tempo da partida
        //m_minimumPlayers = 1; //define pra teste, pode ter opção do dono da sala escolher se n der muito trabalho
        //waitToNextScene = 3f;
        endText.enabled = false;
        m_endMatch = false;
    }

    // Update is called once per frame
    void Update () {
        //Se o evento de countdown ja tiver ativo retorna
        if (eventTriggered) {
            return;
        }

        //chama o evento de fim da partida
        if (endMatch) {
            waitToNextScene -= Time.deltaTime;
            OnTimeOver.Invoke();
        }

        //Chama o evento se tiver o minimo de players
        if (PhotonNetwork.playerList.Length >= minimumPlayers && !endMatch) {
            OnMinimumPlayersReached.Invoke();
            
            eventTriggered = true;
        }
    }

    /// <summary>
    /// Corotina pro tempo não ficar somando no update, é ativada com o evento
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDownRoutine() {

        while (true) {
            yield return new WaitForSeconds(1f);
            m_currentTime++;

            if (m_currentTime >= matchTime) {
                eventTriggered = false;
                m_endMatch = true;
                break;
            }
        }
    }

    public void CountDown() {
        StartCoroutine("CountDownRoutine");
        // m_currentTime += Time.deltaTime;
    }

    public void HandleEndMatch() {
        OnMinimumPlayersReached -= CountDown;
        endText.enabled = true;
        if (waitToNextScene <= 0) {            
            SceneManager.LoadScene(nextScene);
            //OnTimeOver -= HandleEndMatch;
        }
        Debug.Log("ACABOU");
    }
}
