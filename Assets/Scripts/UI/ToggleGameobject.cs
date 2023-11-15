using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameobject : MonoBehaviour
{
    [SerializeField]
    GameObject targetObject;

    private void Update()
    {
        if (targetObject == null)
            return;

        if (UIManager.Instance._OpendUICount != 0)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.SetTab(targetObject, true);
        }
    }
}
