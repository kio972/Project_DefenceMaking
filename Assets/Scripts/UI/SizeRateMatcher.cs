using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SizeRateMatcher : MonoBehaviour
{
    [SerializeField]
    RectTransform target;
    RectTransform myRect;

    [Space]
    [SerializeField]
    private bool matchX = false;
    [SerializeField]
    private bool matchY = false;
    [SerializeField]
    private float rate = 1;
    [Space]
    [SerializeField]
    private Vector2 minSize;
    [SerializeField]
    private Vector2 padding;

    Vector2 curSize;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (target == null || target.sizeDelta == curSize)
            return;

        curSize = target.sizeDelta;
        myRect.sizeDelta = new Vector2(matchX ? Mathf.Max((curSize.x - padding.x) * rate, minSize.x) : myRect.sizeDelta.x,
            matchY ? Mathf.Max((curSize.y - padding.y) * rate, minSize.y) : myRect.sizeDelta.y);
    }
}
