using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public Texture2D fading;
    private float fadeSpeed = 0.8f, drawDepth = -1000f, alpha = 1.0f, fadeDir = -1;

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = (int)drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fading);
    }

    public float BeginFade(float dir)
    {
        fadeDir = dir;
        return fadeSpeed;
    }


}