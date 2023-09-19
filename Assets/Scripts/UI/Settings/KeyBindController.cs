using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindController : MonoBehaviour
{
    // 1. 변경할 키 버튼을 누름
    // 2. 키입력 대기
    // 3. KeyCode.None이 아니라면, 할당 불가 키에 해당하는지 확인. => 할당 불가 키 리스트 작성 필요
    // 4. 해당 키코드가 이미 사용중인지 확인, 만약 사용중이라면 교체

    [SerializeField]
    private List<KeyBinder> keyBinders = new List<KeyBinder>();

    private bool initState = false;

    private KeyBinder ContainsKey(KeyCode value)
    {
        foreach(KeyBinder keyBinder in keyBinders)
        {
            if (keyBinder._CurKeyCode == value)
                return keyBinder;
        }

        return null;
    }

    public void SwapKeyCheck(KeyCode checkKey, KeyCode changeKey)
    {
        if (!initState)
            Init();

        // 현재 checkKey 값을 사용하는 KeyBinder가 있는지 확인
        KeyBinder keyBinder = ContainsKey(checkKey);
        if (keyBinder == null)
            return;
        // value 값을 사용하는 ControlKey가 있는 경우 changeKey로 변경
        keyBinder.SetKey(changeKey);
    }

    private void Init()
    {
        KeyBinder[] keyBinders = GetComponentsInChildren<KeyBinder>();

        foreach(KeyBinder keyBinder in keyBinders)
        {
            if (keyBinder._ControlKey == ControlKey.None)
                continue;

            this.keyBinders.Add(keyBinder);
        }

        initState = true;
    }
}

