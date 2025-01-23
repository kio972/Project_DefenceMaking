using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRaycastAlphaSetting : MonoBehaviour
{
    [SerializeField]
    private float targetThreshold = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = targetThreshold;
    }
}
