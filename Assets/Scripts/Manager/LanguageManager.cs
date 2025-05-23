using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class LanguageManager : Singleton<LanguageManager>
{
    List<Action> languageActions;

    public int curSceneIndex = -1;

    private void ChangeLanguage()
    {
        foreach(var action in languageActions)
        {
            action.Invoke();
        }
    }

    public void RemoveLanguageAction(Action action)
    {
        languageActions.Remove(action);
    }

    public void AddLanguageAction(Action action)
    {
        if (curSceneIndex != SceneManager.GetActiveScene().buildIndex)
            InitLanguageActions();

        languageActions.Add(action);
    }

    private void InitLanguageActions()
    {
        languageActions = new List<Action>();
        curSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Init()
    {
        InitLanguageActions();
        SettingManager.Instance._language.Subscribe(_ => ChangeLanguage()).AddTo(gameObject);
    }
}
