using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCanvas : MonoBehaviour
{
    private static SettingCanvas instance;

    public static SettingCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SettingCanvas>();

                if (instance == null)
                {
                    SettingCanvas prefab = Resources.Load<SettingCanvas>("Prefab/UI/SettingCanvas");
                    instance = Instantiate(prefab);
                    instance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                }
            }

            return instance;
        }
    }

    [SerializeField]
    private GameObject main;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CallSettings(bool value)
    {
        main.SetActive(value);
        if(!value)
        {
            RotationEffect rotationEffect = FindObjectOfType<RotationEffect>();
            if (rotationEffect != null)
                rotationEffect.SetDefault();
        }
    }
}
