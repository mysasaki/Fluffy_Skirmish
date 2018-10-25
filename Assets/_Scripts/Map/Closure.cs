using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Closure : MonoBehaviour {

    public int id;

    [SerializeField]
    private GameObject cross;
    [SerializeField]
    private GameObject circle;

    //private enum State { Open, Closed, ToClose };

    public void Closed() {
        //cross.SetActive(false);
        circle.SetActive(true);
    }

    public void ToClose() {       
        cross.SetActive(true);
        //circle.SetActive(false);
    }

    public void Open() {
        cross.SetActive(false);
        circle.SetActive(false);
    }
}
