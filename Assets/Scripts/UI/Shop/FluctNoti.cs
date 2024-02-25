using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class FluctNoti : MonoBehaviour
{
    [SerializeField]
    Image fluctImg;

    [SerializeField]
    TextMeshProUGUI fluctText;

    [SerializeField]
    private Sprite increaseSprite;
    [SerializeField]
    private Sprite decreaseSprite;

    public void SetNoti(float fluctVal, float originVal)
    {
        gameObject.SetActive(fluctVal != 0);

        if (fluctVal > 0)
        {
            fluctImg.sprite = increaseSprite;
            fluctText.color = Color.green;
        }
        else
        {
            fluctImg.sprite = decreaseSprite;
            fluctText.color = Color.red;
        }

        fluctText.text = Mathf.RoundToInt(Mathf.Abs(fluctVal) / originVal * 100).ToString() + "%";
    }
}
