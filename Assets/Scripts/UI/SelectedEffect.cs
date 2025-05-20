using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject targetEffect;

    public void SetSelected(bool value)
    {
        if (targetEffect == null) return;

        targetEffect.SetActive(value);
    }
}
