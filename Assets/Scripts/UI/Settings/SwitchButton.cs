using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    private Button on_Btn;
    [SerializeField]
    private Button off_Btn;

    [SerializeField]
    private GameObject on_Active;
    [SerializeField]
    private GameObject on_DeActive;
    [SerializeField]
    private GameObject off_Active;
    [SerializeField]
    private GameObject off_DeActive;

    protected bool value;

    protected virtual void OnValueChange()
    {

    }

    private void OnBtn()
    {
        if (value)
            return;

        SetBtn(true);
        OnValueChange();
    }

    private void OffBtn()
    {
        if (!value)
            return;

        SetBtn(false);
        OnValueChange();
    }

    protected void SetBtn(bool value)
    {
        on_Active.SetActive(value);
        on_DeActive.SetActive(!value);
        off_Active.SetActive(!value);
        off_DeActive.SetActive(value);
        this.value = value;
    }

    protected virtual void Init()
    {
        if (on_Btn != null)
            on_Btn.onClick.AddListener(OnBtn);
        if (off_Btn != null)
            off_Btn.onClick.AddListener(OffBtn);
    }

    private void Awake()
    {
        Init();
    }
}
