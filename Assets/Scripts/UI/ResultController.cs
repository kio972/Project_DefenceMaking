using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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
        UtilHelper.IColorEffect(titleBtn, Color.clear, Color.white, 0.5f).Forget();
    }

    private void FadeOn()
    {
        fade.gameObject.SetActive(true);
        UtilHelper.IColorEffect(fade.transform, Color.clear, new Color(0, 0, 0, 0.9f), 2f, () => TitleBtnOn()).Forget();
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
