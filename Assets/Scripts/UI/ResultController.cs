using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Transform victory;
    [SerializeField]
    private Transform defeat;

    [SerializeField]
    private Image fade;

    [SerializeField]
    private Transform titleBtn;

    private void TitleBtnOn()
    {
        titleBtn.gameObject.SetActive(true);
        StartCoroutine(UtilHelper.IColorEffect(titleBtn, Color.clear, Color.white, 0.5f));
    }

    private void FadeOn()
    {
        fade.gameObject.SetActive(true);
        StartCoroutine(UtilHelper.IColorEffect(fade.transform, Color.clear, new Color(0, 0, 0, 0.9f), 2f, () => { TitleBtnOn(); }));
    }

    public void GameWin()
    {
        FadeOn();
        victory.gameObject.SetActive(true);
        defeat.gameObject.SetActive(false);
    }

    public void GameDefeat()
    {
        FadeOn();
        victory.gameObject.SetActive(false);
        defeat.gameObject.SetActive(true);
    }
}
