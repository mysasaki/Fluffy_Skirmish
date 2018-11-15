using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperUI : MonoBehaviour {

    public GameObject container;
    public Text helperText;

    private void SetText(string txt) {
        helperText.text = txt;
    }

    public void DisableHelper() {
        container.SetActive(false);
    }

    public void EnableHelper(HelperType type) {
        string txt = "";

        switch (type) {
            case HelperType.PickWeapon:
                break;

            case HelperType.DropWeapon:
                break;

            case HelperType.PickAmmo:
                break;

            default:
                Debug.Log("Error happened in EnableHelper");
                break;
        }

        container.SetActive(true);
        SetText(txt);
    }
}

public enum HelperType {
    PickWeapon, DropWeapon, PickAmmo
}
