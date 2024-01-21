using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUICallBtn : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Transform targetTransform;
    private PopUIControl popUIControl;

    private void CallPopUpUI()
    {
        if (button == null)
            return;

        if(popUIControl == null)
            popUIControl = GetComponentInParent<PopUIControl>();

        if (popUIControl == null)
            return;

        popUIControl.SetUI(targetTransform);
    }

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(CallPopUpUI);
    }
}
