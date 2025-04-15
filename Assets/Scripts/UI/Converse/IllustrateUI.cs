using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine.Unity.Examples;

public class IllustrateUI : MonoBehaviour
{
    private SkeletonGraphic illust_spine;
    private Image illust_image;
    private SkeletonGraphicRenderTexture illust_renderTexture;

    private Coroutine colorCoroutine;

    private bool initState = false;

    public bool fadeInState = false;

    private SkeletonGraphic _Illust_spine
    {
        get
        {
            if (illust_spine == null)
                illust_spine = GetComponent<SkeletonGraphic>();
            return illust_spine;
        }
    }

    public void PlayTalkAnimation(float talkTime)
    {
        if (_Illust_spine == null)
            return;

        var anim = _Illust_spine.SkeletonData.FindAnimation("talk_start");
        if (anim == null)
            return;
        Spine.TrackEntry track = _Illust_spine.AnimationState.GetCurrent(2);
        _Illust_spine.AnimationState.SetAnimation(2, "talk_start", true);
        _Illust_spine.AnimationState.AddAnimation(2, "talk_end", false, talkTime - anim.Duration);
    }

    public void SetRotation(bool isRight)
    {
        transform.rotation = isRight ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 180, 0));
    }

    public void SetPosition(float x)
    {
        float positionX = x / 1920 * SettingManager.Instance.GetScreenSize()[0];
        transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
    }

    public void SetAnim(string animName, bool loop, int trackNum = 0, string nextAnim = "")
    {
        if (_Illust_spine == null)
            return;

        Spine.TrackEntry track = _Illust_spine.AnimationState.GetCurrent(trackNum);
        if (track == null || track.Animation.Name != animName)
            _Illust_spine.AnimationState.SetAnimation(trackNum, animName, loop);
        if(nextAnim != "")
            _Illust_spine.AnimationState.AddAnimation(trackNum, nextAnim, true, 0f);
    }

    private IEnumerator SetAlphaColor(SkeletonGraphic graphic, Color targetColor, float lerpTime = 0.2f, System.Action callback = null)
    {
        SkeletonGraphicRenderTexture image = graphic.GetComponent<SkeletonGraphicRenderTexture>();
        image.enabled = true;

        Color originColor = image.color;

        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            image.color = Color.Lerp(originColor, targetColor, elapsedTime / lerpTime);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        image.color = targetColor;
        image.enabled = false;
        callback?.Invoke();
    }

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
            illust_renderTexture.color = new Color(1, 1, 1, 1f);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetAlphaColor(illust_spine, new Color(1, 1, 1, 0), 0.2f, () => { illust_spine.gameObject.SetActive(false); }));
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
            illust_renderTexture.color = new Color(1, 1, 1, 0f);
            illust_spine.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetAlphaColor(illust_spine, new Color(1, 1, 1, 1)));
        }
        else if (illust_image != null)
        {
            illust_image.color = new Color(illust_image.color.r, illust_image.color.g, illust_image.color.b, 0f);
            illust_image.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, new Color(illust_image.color.r, illust_image.color.g, illust_image.color.b, 1f)));
        }
    }

    public void SetAlpha(float alpha, float lerpTime = 0.1f)
    {
        if (!initState)
            Init();

        if (!fadeInState)
            return;

        if (illust_spine != null)
        {
            illust_spine.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_spine, new Color(illust_spine.color.r, illust_spine.color.g, illust_spine.color.b, alpha), lerpTime));
        }
        else if (illust_image != null)
        {
            illust_image.gameObject.SetActive(true);
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SetColor(illust_image, new Color(illust_image.color.r, illust_image.color.g, illust_image.color.b, alpha), lerpTime));
        }
    }

    public void SetColor(Color color)
    {
        if (!initState)
            Init();

        if (illust_spine != null)
        {
            illust_spine.color = new Color(color.r, color.g, color.b, illust_spine.color.a);
        }
        else if (illust_image != null)
            illust_image.color = new Color(color.r, color.g, color.b, illust_image.color.a);
    }

    public void ChangeColor(Color color, float lerpTime = 0.1f)
    {
        if (!initState)
            Init();

        if (!fadeInState)
            return;

        if (illust_spine != null)
        {
            illust_spine.gameObject.SetActive(true);
            StartCoroutine(SetColor(illust_spine, color, lerpTime));
        }
        else if (illust_image != null)
        {
            illust_image.gameObject.SetActive(true);
            StartCoroutine(SetColor(illust_image, color, lerpTime));
        }
    }

    private void Init()
    {
        illust_spine = GetComponent<SkeletonGraphic>();
        illust_image = GetComponent<Image>();
        illust_renderTexture = GetComponent<SkeletonGraphicRenderTexture>();
        if(illust_renderTexture != null)
            illust_renderTexture.enabled = false;
        initState = true;
    }
}
