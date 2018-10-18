using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour {

    public Text txt_timer;
    private int aux_timer;

    void Update () {
        aux_timer = (int)TerrainManager.timeToNextClose;
        txt_timer.text = aux_timer.ToString();
    }
}
