using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RotateImg : MonoBehaviour
{
    [SerializeField]
    RectTransform imgRect;

    public float rotationSpeed = 10;

    public bool reverseDircetion = false;

    public void SetDefault()
    {
        imgRect.rotation = Quaternion.identity;
    }

    private void RotateImage()
    {
        float rotationAngle = rotationSpeed * Time.deltaTime;
        if (reverseDircetion)
            rotationAngle *= -1f;
        imgRect.Rotate(0f, 0f, rotationAngle);
    }

    private void OnEnable()
    {
        SetDefault();
    }

    private void Update()
    {
        if (imgRect == null)
            return;

        RotateImage();
    }

}
