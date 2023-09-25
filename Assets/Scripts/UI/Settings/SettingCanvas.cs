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
    [SerializeField]
    private GameObject exitGame;
    [SerializeField]
    private GameObject fade;

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

    public void CallSettings(bool value, bool isIngame)
    {
        exitGame.SetActive(isIngame);

        CallSettings(value);
    }

    public void CallSettings(bool value)
    {
        fade.SetActive(value);
        main.SetActive(value);

        if(!value)
        {
            exitGame.SetActive(false);
            RotationEffect rotationEffect = FindObjectOfType<RotationEffect>();
            if (rotationEffect != null)
                rotationEffect.SetDefault();
        }
    }
}
