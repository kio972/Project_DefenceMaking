using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class IllustrateUI : MonoBehaviour
{
    private SkeletonGraphic illust_spine;
    private Image illust_image;

    private Coroutine colorCoroutine;

    private bool initState = false;

    private IEnumerator SetColor(SkeletonGraphic image, Color targetColor, float lerpTime = 0.2f, System.Action callback = null)
    {
        yield return null;

        Color originColor = image.color;

        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            image.color = Color.Lerp(originColor, targetColor, elapsedTime / lerpTime);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        image.color = targetColor;
        callback?.Invoke();
    }

    private IEnumerator SetColor(Image image, Color targetColor, float lerpTime = 0.2f, System.Action callback = null)
    {
        yield return null;

        Color originColor = image.color;

        float elapsedTime = 0f;
        while(elapsedTime < lerpTime)
        {
            image.color = Color.Lerp(originColor, targetColor, elapsedTime / lerpTime);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        image.color = targetColor;
        callback?.Invoke();
    }

    public void FadeOut()
    {
        if (!initState)
            Init();

        if (illust_spine != null)
        {
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, Color.clear, 0.2f, () => { illust_spine.gameObject.SetActive(false); }));
        }
        else if (illust_image != null)
        {
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, Color.clear, 0.2f, () => { illust_image.gameObject.SetActive(false); }));
        }
    }

    public void FadeIn()
    {
        if (!initState)
            Init();

        if (illust_spine != null)
        {
            illust_spine.color = new Color(illust_image.color.r, illust_image.color.g, illust_image.color.b, 0f);
            illust_spine.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_spine, Color.white));
        }
        else if (illust_image != null)
        {
            illust_image.color = new Color(illust_image.color.r, illust_image.color.g, illust_image.color.b, 0f);
            illust_image.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, Color.white));
        }
    }

    public void SetColor(Color color)
    {
        if (!initState)
            Init();

        if (illust_spine != null)
        {
            illust_spine.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_spine, color, 0.1f));
        }
        else if (illust_image != null)
        {
            illust_image.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, color, 0.1f));
        }
    }

    private void Init()
    {
        illust_spine = GetComponent<SkeletonGraphic>();
        illust_image = GetComponent<Image>();
        initState = true;
    }
}
