using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [SerializeField]
    private Transform levelMidPoint = null;

    [Range(0, 100)]
    private float fogPlaneHeight = 1;
    [SerializeField]
    private Material fogPlaneMaterial = null;

    private GameObject fogPlane = null;
    [SerializeField]
    private Texture2D fogPlaneTextureLerpTarget = null;
    [SerializeField]
    private Texture2D fogPlaneTextureLerpBuffer = null;

    [SerializeField]
    [Range(1, 300)]
    private int levelDimensionX = 11;
    [SerializeField]
    [Range(1, 300)]
    private int levelDimensionY = 11;
    [SerializeField]
    private float unitScale = 1;

    [SerializeField]
    [Range(1, 30)]
    private float FogRefreshRate = 10;
    [SerializeField]
    [Range(1, 5)]
    private float fogLerpSpeed = 2.5f;
    private float FogRefreshRateTimer = 0;

    private void InitializeFog()
    {
        fogPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        fogPlane.name = "[RUNTIME] Fog_Plane";

        fogPlane.transform.position = new Vector3(
            levelMidPoint.position.x,
            levelMidPoint.position.y + fogPlaneHeight,
            levelMidPoint.position.z);

        fogPlane.transform.localScale = new Vector3(
            (levelDimensionX * unitScale) / 10.0f,
            1,
            (levelDimensionY * unitScale) / 10.0f);

        fogPlaneTextureLerpTarget = new Texture2D(levelDimensionX, levelDimensionY);
        fogPlaneTextureLerpBuffer = new Texture2D(levelDimensionX, levelDimensionY);

        fogPlaneTextureLerpBuffer.wrapMode = TextureWrapMode.Clamp;

        fogPlaneTextureLerpBuffer.filterMode = FilterMode.Bilinear;

        fogPlane.GetComponent<MeshRenderer>().material = new Material(fogPlaneMaterial);

        fogPlane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);

        fogPlane.GetComponent<MeshCollider>().enabled = false;
    }

    private void UpdateFogPlaneTextureBuffer()
    {
        for (int xIterator = 0; xIterator < levelDimensionX; xIterator++)
        {
            for (int yIterator = 0; yIterator < levelDimensionY; yIterator++)
            {
                Color bufferPixel = fogPlaneTextureLerpBuffer.GetPixel(xIterator, yIterator);
                Color targetPixel = fogPlaneTextureLerpTarget.GetPixel(xIterator, yIterator);

                fogPlaneTextureLerpBuffer.SetPixel(xIterator, yIterator, Color.Lerp(
                    bufferPixel,
                    targetPixel,
                    fogLerpSpeed * Time.deltaTime));
            }
        }

        fogPlaneTextureLerpBuffer.Apply();
    }

    private void UpdateFogField()
    {
        //shadowcaster.ResetTileVisibility();

        //foreach (FogRevealer fogRevealer in fogRevealers)
        //{
        //    fogRevealer.GetCurrentLevelCoordinates(this);

        //    shadowcaster.ProcessLevelData(
        //        fogRevealer._CurrentLevelCoordinates,
        //        Mathf.RoundToInt(fogRevealer._SightRange / unitScale));
        //}

        //UpdateFogPlaneTextureTarget();
    }

    private void UpdateFog()
    {
        fogPlane.transform.position = new Vector3(
            levelMidPoint.position.x,
            levelMidPoint.position.y + fogPlaneHeight,
            levelMidPoint.position.z);

        FogRefreshRateTimer += Time.deltaTime;

        if (FogRefreshRateTimer < 1 / FogRefreshRate)
        {
            UpdateFogPlaneTextureBuffer();

            return;
        }
        else
        {
            // This is to cancel out minor excess values
            FogRefreshRateTimer -= 1 / FogRefreshRate;
        }

        //foreach (FogRevealer fogRevealer in fogRevealers)
        //{
        //    if (fogRevealer._UpdateOnlyOnMove == false)
        //    {
        //        break;
        //    }

        //    Vector2Int currentLevelCoordinates = fogRevealer.GetCurrentLevelCoordinates(this);

        //    if (currentLevelCoordinates != fogRevealer._LastSeenAt)
        //    {
        //        break;
        //    }

        //    if (fogRevealer == fogRevealers.Last())
        //    {
        //        return;
        //    }
        //}

        UpdateFogField();

        UpdateFogPlaneTextureBuffer();
    }
}
