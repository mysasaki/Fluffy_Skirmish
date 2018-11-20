using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperUI : MonoBehaviour {

    public GameObject container;
    public Text helperText;

    private void Start() {
        HideHelper();
    }

    private void SetText(string txt) {
        helperText.text = txt;
    }

    public void HideHelper() {
        container.SetActive(false);
    }

    public void ShowHelper() {
        container.SetActive(true);
        SetText("Press 'E' to equip weapon");
    }
}

// Nao vai precisar mas vai que
/*public enum HelperType {
    PickWeapon, PickAmmo
}*/
