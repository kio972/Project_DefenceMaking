using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideSpiner : SpinnerButton
{
    [SerializeField]
    List<Image> guideBtns;

    [SerializeField]
    List<GameObject> guideInfos;

    [SerializeField]
    GameObject mainInfoMenu;

    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite selectedSprite;

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(gameObject, value, () => { mainInfoMenu?.SetActive(true); });
    }

    public void SetIndex(int index)
    {
        guideBtns[this.index].sprite = defaultSprite;
        this.index = index;
        guideBtns[this.index].sprite = selectedSprite;
        SetBtn();
        OnValueChange();
    }

    protected override void OnValueChange()
    {
        foreach(GameObject info in guideInfos)
            info.SetActive(false);

        guideInfos[index].SetActive(true);
    }

    private void OnDisable()
    {
        SetIndex(0);
    }

    private void OnEnable()
    {
        SetIndex(0);
    }
}
