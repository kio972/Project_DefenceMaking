using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UniRx;

public class ResearchTreePrevLine : MonoBehaviour
{
    private UILineRenderer uiLineRenderer;

    [SerializeField]
    private Transform activeLines;
    private Transform targetLine;

    [SerializeField]
    private ResearchSlot targetResearch;

    private void Awake()
    {
        if (targetResearch == null)
            return;

        uiLineRenderer = GetComponent<UILineRenderer>();
        targetLine = activeLines.transform.GetChild(transform.GetSiblingIndex());

        targetResearch._curState.Subscribe(_ => targetLine.gameObject.SetActive(_ != ResearchState.Impossible && _ != ResearchState.None)).AddTo(gameObject);
    }
}
