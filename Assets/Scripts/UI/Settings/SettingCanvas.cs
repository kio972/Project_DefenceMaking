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
    private GameObject fade;

    [SerializeField]
    private GameObject ingameMenu;

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

    public void CloseSettingTab()
    {
        if (ingameMenu.activeSelf)
            UIManager.Instance.SetTab(main, false);
        else
            CallSettings(false);
    }

    public void CallSettings(bool value, bool isIngame = false)
    {
        if(value)
        {
            Time.timeScale = 0;
            GameManager.Instance.isPause = true;
            UIManager.Instance.SetTab(ingameMenu, isIngame);
            UIManager.Instance.SetTab(main, !isIngame);
        }
        else
        {
            Time.timeScale = 1;
            GameManager.Instance.isPause = false;
            UIManager.Instance.SetTab(ingameMenu, false);
            UIManager.Instance.SetTab(main, false);
            InGameSettingCallBtn rotationEffect = FindObjectOfType<InGameSettingCallBtn>();
            rotationEffect?.SetDefault();
        }

        fade.SetActive(value);
    }

    public void Update()
    {
        if(fade.activeSelf)
        {
            if (!main.activeSelf && !ingameMenu.activeSelf)
                CallSettings(false);
        }
    }
}
