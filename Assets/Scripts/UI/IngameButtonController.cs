using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameButtonController : MonoBehaviour
{
    [SerializeField]
    private Button shopBtn;
    [SerializeField]
    private Button skipBtn;

    [SerializeField]
    private SkipDoorBtn skipBtnAnim;

    private void SkipWave()
    {
        GameManager.Instance.SkipDay();
    }

    private void OpenShop()
    {
        GameManager.Instance.speedController.SetSpeedZero();
    }

    private void SetSkipBtnAvail()
    {
        if (skipBtn == null)
            return;

        if (GameManager.Instance.waveController.WaveProgress >= 1 && GameManager.Instance.adventurersList.Count == 0)
            skipBtn.gameObject.SetActive(true);
        else
            skipBtn.gameObject.SetActive(false);
    }

    private void SetShopBtnAvail()
    {
        if (shopBtn == null)
            return;

        if (GameManager.Instance.Timer >= 720f)
            shopBtn.gameObject.SetActive(true);
        else
        {
            skipBtnAnim.ResetAnimation();
            shopBtn.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        if (shopBtn != null)
            shopBtn.onClick.AddListener(OpenShop);
        if(skipBtn != null)
            skipBtn.onClick.AddListener(SkipWave);
    }

    private void Update()
    {
        //SetShopBtnAvail();
        SetSkipBtnAvail();
    }
}
