using FischlWorks_FogWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static FischlWorks_FogWar.csFogWar;
using static System.Net.Mime.MediaTypeNames;

public class FogOfWar : MonoBehaviour
{
    [SerializeField]
    private Camera fogCam;
    [SerializeField]
    private RenderTexture texture;
    [SerializeField]
    private Material fogPlaneMaterial;

    private GameObject fogPlane;

    [SerializeField]
    private Texture2D fogPlaneTextureLerpTarget;
    [SerializeField]
    private Texture2D fogPlaneTextureLerpBuffer;

    [SerializeField]
    private Transform planeTransform;

    [SerializeField]
    private float fogPlaneHeight = 0;

    [SerializeField]
    private Color fogColor = Color.black;

    private void InitializeFog()
    {
        fogPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        fogPlane.name = "[RUNTIME] Fog_Plane";

        fogPlane.transform.position = new Vector3(
            planeTransform.position.x,
            planeTransform.position.y + fogPlaneHeight,
            planeTransform.position.z);

        fogPlane.transform.SetParent(transform);

        fogPlane.transform.localScale = planeTransform.localScale;

        fogPlaneTextureLerpTarget = new Texture2D(texture.width, texture.height);
        fogPlaneTextureLerpBuffer = new Texture2D(texture.width, texture.height);

        fogPlaneTextureLerpBuffer.wrapMode = TextureWrapMode.Clamp;
        fogPlaneTextureLerpBuffer.filterMode = FilterMode.Bilinear;

        fogPlane.GetComponent<MeshRenderer>().material = new Material(fogPlaneMaterial);

        fogPlane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", fogPlaneTextureLerpTarget);

        fogPlane.GetComponent<MeshCollider>().enabled = false;
    }

    Color[] color;

    private Color[] GetColors(Texture2D renderTexture)
    {
        if(color == null)
            color = new Color[renderTexture.width * renderTexture.height];
        for (int height = 0; height < renderTexture.height; height++)
        {
            for(int width = 0; width < renderTexture.width; width++)
            {
                //float pixelAlpha = renderTexture.GetPixel(width, height);
                color[height * width + width] = renderTexture.GetPixel(width, height);
            }
        }

        return color;
    }

    private void UpdateFogField()
    {
        fogCam.Render();
        RenderTexture.active = texture;
        //Texture2D texture2D = new Texture2D(texture.width, texture.height);
        //Graphics.Blit(fogPlaneTextureLerpTarget, texture);
        fogPlaneTextureLerpTarget.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        //texture2D.Apply();

        for(int i = 0; i < fogPlaneTextureLerpTarget.width; i++)
        {
            for(int j = 0; j < fogPlaneTextureLerpTarget.height; j++)
            {
                Color pixelColor = fogPlaneTextureLerpTarget.GetPixel(0, 0);
                fogPlaneTextureLerpTarget.SetPixel(i, j, new Color(1, 1, 1, 1 - pixelColor.a));
            }
        }

        //fogPlaneTextureLerpTarget.SetPixels(GetColors(texture2D));
        fogPlaneTextureLerpTarget.Apply();
    }

    private void Start()
    {
        InitializeFog();
        UpdateFogField();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
            UpdateFogField();
    }
}
