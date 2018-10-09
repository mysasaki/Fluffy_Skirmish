using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillListing : MonoBehaviour {

    public Text killText;

    public void UpdateText(string newText) {
        this.killText.text = newText;
    }
}
