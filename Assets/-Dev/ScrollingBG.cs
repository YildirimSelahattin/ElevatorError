using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    public float bgSpeed = 0.02f;
    public Renderer bgRend;
    public static int breake = 1;

    void Update()
    {
        bgRend.materials[0].mainTextureOffset += new Vector2(breake * bgSpeed * Time.deltaTime, 0f);
        if (bgRend.materials.Length > 1)
        {
            bgRend.materials[1].mainTextureOffset += new Vector2(breake * bgSpeed * Time.deltaTime, 0f);
        }

    }
}