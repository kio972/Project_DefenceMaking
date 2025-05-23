using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindController : MonoBehaviour
{
    // 1. ������ Ű ��ư�� ����
    // 2. Ű�Է� ���
    // 3. KeyCode.None�� �ƴ϶��, �Ҵ� �Ұ� Ű�� �ش��ϴ��� Ȯ��. => �Ҵ� �Ұ� Ű ����Ʈ �ۼ� �ʿ�
    // 4. �ش� Ű�ڵ尡 �̹� ��������� Ȯ��, ���� ������̶�� ��ü

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

        // ���� checkKey ���� ����ϴ� KeyBinder�� �ִ��� Ȯ��
        KeyBinder keyBinder = ContainsKey(checkKey);
        if (keyBinder == null)
            return;
        // value ���� ����ϴ� ControlKey�� �ִ� ��� changeKey�� ����
        keyBinder.SetKey(changeKey);
    }

    private void Init()
    {
        KeyBinder[] keyBinders = GetComponentsInChildren<KeyBinder>(true);

        foreach(KeyBinder keyBinder in keyBinders)
        {
            if (keyBinder._ControlKey == ControlKey.None)
                continue;

            this.keyBinders.Add(keyBinder);
        }

        initState = true;
    }
}

