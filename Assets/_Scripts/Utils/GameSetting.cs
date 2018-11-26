using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour {

    public static GameSetting Instance = null;
    public Slider slider;
    public MatchTime matchTime;

    public void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void Start() {
        matchTime = MatchTime.Short;
    }

    public void OnSlide_ChangedValue() {

        if (!PhotonNetwork.isMasterClient)
            return;

        int value = (int)slider.value;
        switch(value) {
            case 1:
                matchTime = MatchTime.Short;
                break;

            case 2:
                matchTime = MatchTime.Medium;
                break;

            case 3:
                matchTime = MatchTime.Long;
                break;

            default:
                matchTime = MatchTime.Short;
                break;
        }
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}
