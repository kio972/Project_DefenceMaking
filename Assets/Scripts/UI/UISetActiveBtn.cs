using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetActiveBtn : MonoBehaviour
{
    [SerializeField]
    protected GameObject targetObject;

    public virtual void SetActive(bool value)
    {
        if (value)
            UIManager.Instance.AddTab(targetObject);
        else
            UIManager.Instance.CloseTab(targetObject);
    }
}
