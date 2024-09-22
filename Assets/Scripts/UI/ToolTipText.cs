using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipText : MonoBehaviour
{
    TextMeshProUGUI toolTipText;

    private string[] toolTipEx = { "���� 0�ÿ� ħ���մϴ�.", "��� ������ �������κ��� ��������� �ʽ��ϴ�.", "��Ż���� ���ռ��� �ΰ� ������ ���� ���� �����մϴ�.", "��Ż������ ��� �켱���� �������Դϴ�." };
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
