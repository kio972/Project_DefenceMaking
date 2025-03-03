using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UniRx;

public class ResearchTreeSelectLine : MonoBehaviour
{
    private UILineRenderer uiLineRenderer;

    [SerializeField]
    private Transform activeLines;
    private Transform targetLine;

    [SerializeField]
    private ResearchSlot[] targetResearchs;

    private bool IsActive
    {
        get
        {
            foreach(var research in targetResearchs)
            {
                if (research._CurState == ResearchState.Complete)
                    return true;
            }

            return false;
        }
    }

    private void Awake()
    {
        if (targetResearchs == null || targetResearchs.Length == 0)
            return;

        uiLineRenderer = GetComponent<UILineRenderer>();
        targetLine = activeLines.transform.GetChild(transform.GetSiblingIndex());

        foreach(ResearchSlot targetResearch in targetResearchs)
            targetResearch._curState.Subscribe(_ => targetLine.gameObject.SetActive(IsActive)).AddTo(gameObject);
    }
}
