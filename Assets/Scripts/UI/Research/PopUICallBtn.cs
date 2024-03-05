using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUICallBtn : MonoBehaviour
{
    [SerializeField]
    protected Button button;
    [SerializeField]
    protected Transform targetTransform;
    private PopUIControl popUIControl;

    public virtual void CallPopUpUI()
    {
        if (button == null)
            return;

        if(popUIControl == null)
            popUIControl = GetComponentInParent<PopUIControl>();

        if (popUIControl == null)
            return;

        popUIControl.SetUI(targetTransform);
    }

    protected virtual void Awake()
    {
        if (button != null)
            button.onClick.AddListener(CallPopUpUI);
    }
}
