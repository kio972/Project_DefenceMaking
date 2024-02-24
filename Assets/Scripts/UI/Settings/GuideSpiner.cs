using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSpiner : SpinnerButton
{
    [SerializeField]
    List<GameObject> guideInfos;

    [SerializeField]
    GameObject mainInfoMenu;

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(gameObject, value, () => { mainInfoMenu?.SetActive(true); });
    }

    public void SetIndex(int index)
    {
        this.index = index;
        SetBtn();
        OnValueChange();
    }

    protected override void OnValueChange()
    {
        foreach(GameObject info in guideInfos)
            info.SetActive(false);

        guideInfos[index].SetActive(true);
    }
}
