using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpMessage : MonoBehaviour
{
    [SerializeField]
    Image toastBg;
    [SerializeField]
    TextMeshProUGUI toastText;

    public float toastTime = 0.5f;
    public float colorLerpTime = 1f;

    private Coroutine toastCoroutine;

    WaitForSeconds waitTime = null;

    private bool initState = false;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator IToastMsg()
    {
        toastText.color = new Color(toastText.color.r, toastText.color.g, toastText.color.b, 1f);
        if(toastBg != null)
            toastBg.color = new Color(toastBg.color.r, toastBg.color.g, toastBg.color.b, 1f);

        yield return waitTime;

        float elapsedTime = 0f;

        while (elapsedTime < colorLerpTime)
        {
            elapsedTime += Time.deltaTime;

            float color_A = Mathf.Lerp(1f, 0f, elapsedTime / colorLerpTime);
            toastText.color = new Color(toastText.color.r, toastText.color.g, toastText.color.b, color_A);
            if (toastBg != null)
                toastBg.color = new Color(toastBg.color.r, toastBg.color.g, toastBg.color.b, color_A);
            yield return null;
        }

        toastText.color = new Color(toastText.color.r, toastText.color.g, toastText.color.b, 0f);
        if (toastBg != null)
            toastBg.color = new Color(toastBg.color.r, toastBg.color.g, toastBg.color.b, 0f);

        gameObject.SetActive(false);
    }

    public void ToastMsg(string message)
    {
        if (!initState)
            Init();

        if (toastCoroutine != null)
            StopCoroutine(toastCoroutine);

        gameObject.SetActive(true);
        toastText.text = message;
        toastCoroutine = StartCoroutine(IToastMsg());
    }

    private void Init()
    {
        if (toastBg == null)
            toastBg = GetComponent<Image>();
        if (toastText == null)
            toastText = GetComponentInChildren<TextMeshProUGUI>();
        waitTime = new WaitForSeconds(toastTime);
        initState = true;
    }
}
