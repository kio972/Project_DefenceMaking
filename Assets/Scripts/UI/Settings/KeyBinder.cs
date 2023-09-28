using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyBinder : MonoBehaviour
{
    [SerializeField]
    private Button keyChangeBtn;
    [SerializeField]
    private TextMeshProUGUI keyText;
    [SerializeField]
    private Outline outline;

    [SerializeField]
    private ControlKey controlKey = ControlKey.None;
    public ControlKey _ControlKey { get => controlKey; }

    private KeyCode curKeyCode = KeyCode.None;
    public KeyCode _CurKeyCode { get => curKeyCode; }

    private KeyBindController keyBindController;

    private bool updateState = false;

    public void ChangeKeyDisplay(KeyCode keyCode)
    {
        string targetString = keyCode.ToString();
        targetString = targetString.Replace("KeyCode.", "");
        targetString = targetString.Replace("Alpha", "");
        keyText.text = targetString;
    }

    private void ChangeKeyBtn()
    {
        updateState = true;
        outline.enabled = true;
    }

    public KeyCode GetCurrentKeyDown()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    private void UpdateCheck()
    {
        KeyCode inputKey = GetCurrentKeyDown();
        if (inputKey == KeyCode.None)
            return;

        if (SettingManager.Instance.IsValidKey(inputKey))
        {
            keyBindController?.SwapKeyCheck(inputKey, curKeyCode);
            SetKey(inputKey);
        }

        updateState = false;
        outline.enabled = false;
    }

    public void SetKey(KeyCode keyCode)
    {
        ChangeKeyDisplay(keyCode);
        curKeyCode = keyCode;
        SettingManager.Instance.SetKey(controlKey, curKeyCode);
    }

    private void Init()
    {
        if (keyChangeBtn == null)
            keyChangeBtn = GetComponentInChildren<Button>();
        if (keyText == null)
            keyText = GetComponentInChildren<TextMeshProUGUI>();

        if (keyText == null || keyChangeBtn == null)
            return;

        keyBindController = GetComponentInParent<KeyBindController>();

        keyChangeBtn.onClick.AddListener(ChangeKeyBtn);

        curKeyCode = SettingManager.Instance.GetKey(controlKey);

        ChangeKeyDisplay(curKeyCode);
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (!updateState)
            return;
        UpdateCheck();
    }
}
