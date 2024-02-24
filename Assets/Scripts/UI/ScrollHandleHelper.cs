using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHandleHelper : MonoBehaviour
{
    Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Update()
    {
        if (scrollbar == null || scrollbar.handleRect == null)
            return;

        Vector2 pivot = scrollbar.handleRect.pivot;
        pivot.y = scrollbar.value;
        scrollbar.handleRect.pivot = pivot; 
    }
}
