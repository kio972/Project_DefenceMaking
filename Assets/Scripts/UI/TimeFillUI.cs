using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeFillUI : MonoBehaviour
{
    [SerializeField]
    private Image fillImg;

    private void SetFill()
    {
        float fillRate = GameManager.Instance.Timer / 1440f;

        fillImg.fillAmount = fillRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (fillImg == null)
            return;

        SetFill();
    }
}
