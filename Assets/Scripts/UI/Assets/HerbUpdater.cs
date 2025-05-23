using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;

public class HerbUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI targetText;

    [SerializeField]
    private Image herbBoxFill;
    [SerializeField]
    private GameObject herbMaxImg;

    [SerializeField]
    private HerbType targetHerb;

    private void SetHerbBox(int herbCount, int herbMax)
    {
        if (herbBoxFill == null || herbMaxImg == null)
            return;

        herbMaxImg.SetActive(false);
        if (herbMax == 0 && herbCount > 0)
            herbMaxImg.SetActive(true);
        else if (herbMax > 0 && herbCount > herbMax)
            herbMaxImg.SetActive(true);
        else if (herbMax > 0)
            herbBoxFill.fillAmount = (float)herbCount / (float)herbMax;
        else if (herbMax == 0 && herbCount == 0)
            herbBoxFill.fillAmount = 0;
    }

    private void Start()
    {
        targetText.text = GameManager.Instance.herbDic[targetHerb].ToString();
        GameManager.Instance.herbDic.ObserveReplace().Where(_ => _.Key == targetHerb).Subscribe(_ => targetText.text = _.NewValue.ToString());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    int herbCount = GameManager.Instance.herbDic[HerbType.BlackHerb];
    //    targetText.text = herbCount.ToString();
    //    int herbMax = GameManager.Instance.herb1Max;
    //    SetHerbBox(herbCount, herbMax);
    //}
}
