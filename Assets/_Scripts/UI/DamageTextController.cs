using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour {

    private static GameObject damageText;
    private static GameObject canvas;

    public static void Initialize() {
        canvas = GameObject.Find("Canvas");

        if(!damageText)
            damageText = Resources.Load<GameObject>("Prefabs/DamagePopup");
    }

	public static void CreateDamageText(string text, Transform location) {
        if (!damageText)
            damageText = Resources.Load<GameObject>("Prefabs/DamagePopup");

        if(!canvas)
            canvas = GameObject.Find("Canvas");

        Vector3 newLocation = location.position + new Vector3(0, 7.5f, 0);
        GameObject instance = Instantiate(damageText);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(newLocation);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPos;
        instance.GetComponent<DamageText>().SetText(text);
    }
}
