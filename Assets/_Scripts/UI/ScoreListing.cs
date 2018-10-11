using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListing : MonoBehaviour {

    public int id;
    public Text playerText;
    public Text killText;
    public Text deathText;

    public void InitializeScore(int id, string name, string killText, string deathText) {
        this.id = id;
        this.playerText.text = name;
        this.killText.text = killText;
        this.deathText.text = deathText;
    }

    public void UpdateText(string killText, string deathText) {
        this.killText.text = killText;
        this.deathText.text = deathText;
    }
	
}
