using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public Texture texture;

    private MaterialPropertyBlock material;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            material = new MaterialPropertyBlock();
            material.SetTexture("_MainTex", texture);
            GetComponent<Renderer>().SetPropertyBlock(material);
        }
    }
}
