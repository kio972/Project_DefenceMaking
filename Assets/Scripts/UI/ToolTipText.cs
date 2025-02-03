using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipText : MonoBehaviour
{
    TextMeshProUGUI toolTipText;

    [SerializeField]
    private string[] toolTipEx;
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
        toolTipText.text = DataManager.Instance.GetDescription(toolTipEx[nextIndex]);
    }
}
