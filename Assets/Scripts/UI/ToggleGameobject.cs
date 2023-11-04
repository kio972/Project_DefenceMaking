using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameobject : MonoBehaviour
{
    [SerializeField]
    GameObject targetObject;

    [SerializeField]
    KeyCode targetKey;

    private bool toggleState = false;

    public bool isOneWay = false;
    private bool originState;

    public void SetToggleState(bool value)
    {
        toggleState = value;
        targetObject.SetActive(toggleState);
    }

    public void ToggleState()
    {
        toggleState = !toggleState;
        targetObject.SetActive(toggleState);
    }

    private void Update()
    {
        if (targetObject == null || targetKey == KeyCode.None)
            return;

        if(Input.GetKeyDown(targetKey))
        {
            if (isOneWay && toggleState != originState)
                return;

            ToggleState();
        }
    }

    private void Awake()
    {
        toggleState = targetObject.activeSelf;
        originState = toggleState;
    }
}
