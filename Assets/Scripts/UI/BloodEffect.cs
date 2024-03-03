using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffect : MonoBehaviour
{
    private Image effectImg;

    private float lerpTime = 0.3f;

    private IEnumerator IBloodEffect()
    {
        while(effectImg.color.a < 1)
        {
            float alpha = effectImg.color.a;
            alpha += Time.deltaTime / lerpTime;
            alpha = Mathf.Clamp01(alpha);
            effectImg.color = new Color(effectImg.color.r, effectImg.color.g, effectImg.color.b, alpha);
            yield return null;
        }

        while(effectImg.color.a > 0)
        {
            float alpha = effectImg.color.a;
            alpha -= Time.deltaTime / lerpTime;
            alpha = Mathf.Clamp01(alpha);
            effectImg.color = new Color(effectImg.color.r, effectImg.color.g, effectImg.color.b, alpha);

            yield return null;
        }

        effectImg.color = new Color(effectImg.color.r, effectImg.color.g, effectImg.color.b, 0);
    }

    public void StartBloodEffect()
    {
        StopAllCoroutines();
        StartCoroutine(IBloodEffect());
    }

    private void Awake()
    {
        effectImg = GetComponent<Image>();
    }
}
