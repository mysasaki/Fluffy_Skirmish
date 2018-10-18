using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] [DisallowMultipleComponent]
public class Lava : MonoBehaviour {

    public Vector2 lava1vel;
    public Vector2 lava2vel;
    string textureName = "_MainTex";
    string texture2Name = "_DetailAlbedoMap";

    Material mat;
    Vector2 uvOffset1 = Vector2.up;
    Vector2 uvOffset2 = Vector2.up * 2;

    public Material Mat {
        get {
            if (mat == null) {
                mat = GetComponent<Renderer>().sharedMaterial;
            }
            return mat;
        }
    }

    void Update() {
        if (Mat != null) {
            uvOffset1 += (lava1vel * Time.deltaTime);
            uvOffset2 += (lava2vel * Time.deltaTime);
            Mat.SetTextureOffset(textureName, uvOffset1);
            Mat.SetTextureOffset(texture2Name, uvOffset2);
        }
    }
}
