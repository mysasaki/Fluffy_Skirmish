using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {
    /// <summary>
    /// transform da bussola
    /// </summary>
    private Transform compassTransform;
    /// <summary>
    /// vetor de direção pra fazer a rotação da bussola
    /// </summary>
    private Vector3 dir;

    // Use this for initialization
    private void Start () {
        //acha o gameobject da bussola pra pegar o transform
        GetCompassTransform();
    }

    private void GetCompassTransform() {
        MapMenu mapMenu = FindObjectOfType<MapMenu>();
        compassTransform = mapMenu.compass.transform;
    }
	
	// Update is called once per frame
	private void Update () {
        if (!compassTransform)
            GetCompassTransform();

        //se for o local player, faz a rotação
        if (this.GetComponent<PhotonView>().isMine) {
            dir.z = transform.eulerAngles.y;
            compassTransform.localEulerAngles = dir;
        }

       
	}
}
