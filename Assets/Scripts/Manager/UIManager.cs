using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private List<GameObject> targetUI = new List<GameObject>();

    private Dictionary<GameObject, System.Action> closeActDic = new Dictionary<GameObject, System.Action>();

    public int _OpendUICount { get => targetUI.Count; }

    private bool frameUpdate = false;

    public void CloseTab(GameObject target)
    {
        target.SetActive(false);
        targetUI.Remove(target);
        if (closeActDic.ContainsKey(target))
            closeActDic.Remove(target);
    }

    public void CloseTab()
    {
        GameObject target = targetUI[targetUI.Count - 1];
        if (closeActDic.ContainsKey(target))
            closeActDic[target].Invoke();
        CloseTab(target);
    }

    public void AddTab(GameObject target, System.Action closeAct = null)
    {
        if (target == null)
            return;

        targetUI.Add(target);
        target.SetActive(true);
        if (closeAct != null)
            closeActDic.Add(target, closeAct);
    }

    public void SetTab(GameObject target, bool value, System.Action closeAct = null)
    {
        if (value)
            AddTab(target, closeAct);
        else
            CloseTab(target);
        frameUpdate = false;
    }

    private void LateUpdate()
    {
        if(!frameUpdate)
        {
            frameUpdate = true;
            return;
        }

        if (targetUI.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseTab();
    }
}
