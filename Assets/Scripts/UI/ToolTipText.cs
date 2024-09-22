using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipText : MonoBehaviour
{
    TextMeshProUGUI toolTipText;

    private string[] toolTipEx = { "적은 0시에 침입합니다.", "모든 마왕이 마족으로부터 존경받지는 않습니다.", "단탈리온 마왕성은 인간 나라의 영토 내에 존재합니다.", "단탈리온은 토벌 우선순위 최하위입니다." };
    int prevIndex = -1;

    private void Awake()
    {
        toolTipText = GetComponent<TextMeshProUGUI>();
    }

    public void SetRandomToolTip()
    {
        if (toolTipText == null)
            return;

        int nextIndex = Random.Range(0, toolTipEx.Length);
        while(toolTipEx.Length != 1 && nextIndex == prevIndex)
            nextIndex = Random.Range(0, toolTipEx.Length);
        prevIndex = nextIndex;
        toolTipText.text = toolTipEx[nextIndex];
    }
}
