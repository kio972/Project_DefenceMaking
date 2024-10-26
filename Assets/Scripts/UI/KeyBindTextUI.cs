using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class KeyBindTextUI : MonoBehaviour
{
    TextMeshProUGUI targetText;
    [SerializeField]
    private ControlKey targetKey;

    private BindKey _bindKey;

    private KeyCode _prevKey;

    // Start is called before the first frame update
    void Start()
    {
        targetText = GetComponent<TextMeshProUGUI>();
        _bindKey = SettingManager.Instance.GetBindKey(targetKey);
    }

    private void Update()
    {
        if (_prevKey == _bindKey._CurKey)
            return;
        _prevKey = _bindKey._CurKey;
        targetText.text = _bindKey._CurKey.ToString().Replace("KeyCode.", "").Replace("Alpha", "");
    }
}
