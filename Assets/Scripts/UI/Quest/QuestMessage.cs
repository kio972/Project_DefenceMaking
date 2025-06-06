using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class QuestMessage : MonoBehaviour
{
    [SerializeField]
    private LanguageText titleText;
    [SerializeField]
    private LanguageText messageText;
    [SerializeField]
    private LanguageText select1;
    [SerializeField]
    private LanguageText select2;

    [SerializeField]
    private DissolveController dissolveController;

    private int curIndex = 0;
    private List<string> targetQuest;

    [SerializeField]
    private Image img;
    [SerializeField]
    private Image fade;

    [SerializeField]
    private float fadeAlpha;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Sprite[] mainSelectSprite;
    [SerializeField]
    private Sprite[] subSelectSprite;
    [SerializeField]
    private Image select1Back;
    [SerializeField]
    private Image select2Back;

    private bool closing = false;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    [SerializeField]
    FMODUnity.EventReference openSound;
    [SerializeField]
    FMODUnity.EventReference closeSound;

    private IEnumerator SetAlpha(bool value, float lerpTime, float waitTime = 0)
    {
        TextMeshProUGUI[] textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        Image[] imgs = GetComponentsInChildren<Image>();

        float startVal = value ? 0 : 1;
        float endVal = value ? 1 : 0;
        foreach (TextMeshProUGUI text in textMeshProUGUIs)
            text.color = new Color(text.color.r, text.color.g, text.color.b, startVal);
        foreach(Image img in imgs)
        {
            if (img == this.img || img == fade)
                continue;
            img.color = new Color(img.color.r, img.color.g, img.color.b, startVal);
        }

        float elapsedTime = 0;
        if(waitTime != 0)
        {
            while(elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            elapsedTime = 0;
        }

        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Lerp(startVal, endVal, elapsedTime / lerpTime);
            foreach (TextMeshProUGUI text in textMeshProUGUIs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, t);
            foreach (Image img in imgs)
            {
                if (img == this.img || img == fade)
                    continue;
                img.color = new Color(img.color.r, img.color.g, img.color.b, t);
            }
            yield return null;
        }

        foreach (TextMeshProUGUI text in textMeshProUGUIs)
            text.color = new Color(text.color.r, text.color.g, text.color.b, endVal);
        foreach (Image img in imgs)
        {
            if (img == this.img || img == fade)
                continue;
            img.color = new Color(img.color.r, img.color.g, img.color.b, endVal);
        }
    }

    private IEnumerator Resume(string id)
    {
        //string closeClip = "Quest_Paper_Burn_" + Random.Range(1, 3).ToString();
        //AudioManager.Instance.Play2DSound(closeClip, SettingManager.Instance._FxVolume);
        FMODUnity.RuntimeManager.PlayOneShot(closeSound);

        closing = true;

        UtilHelper.IColorEffect(fade.transform, new Color(0, 0, 0, fadeAlpha), Color.clear, 0.1f, () => fade.gameObject.SetActive(false)).Forget();
        yield return StartCoroutine(SetAlpha(false, 0.1f));
        GameManager.Instance.SetPause(false);

        float targetTime = dissolveController.disappareGoaltime;
        float elapsedTime = 0f;
        while(elapsedTime < targetTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        QuestManager.Instance.StartQuest(id);
        closing = false;
        gameObject.SetActive(false);
    }

    private void StartQuest(string id)
    {
        if (closing)
            return;

        dissolveController.isDisappare = true;
        StartCoroutine(Resume(id));
    }

    public void SelectFirst()
    {
        StartQuest(targetQuest[curIndex]);
    }

    public void SelectSecond()
    {
        if (targetQuest.Count > 2)
        {
            curIndex++;
            if (curIndex > targetQuest.Count - 1)
                curIndex = 0;
        }
        else
            StartQuest(targetQuest[1]);
    }

    private void ResetUIColors()
    {
        TextMeshProUGUI[] textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        Image[] imgs = GetComponentsInChildren<Image>(true);
        foreach (TextMeshProUGUI text in textMeshProUGUIs)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        foreach (Image img in imgs)
        {
            if (img == this.img || img == fade)
                continue;
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
    }

    public async UniTaskVoid SetMessage(Dictionary<string, object> data)
    {
        fade.gameObject.SetActive(true);
        UtilHelper.IColorEffect(fade.transform, Color.clear, new Color(0, 0, 0, fadeAlpha), 0.5f).Forget();
        dissolveController.isAppare = true;
        gameObject.SetActive(true);
        ResetUIColors();
        GameManager.Instance.SetPause(true);

        if (data == null)
            return;

        //titleText.text = data["MessageTitle"].ToString();
        titleText.ChangeLangauge(SettingManager.Instance.language, data["TitleKey"].ToString());
        //messageText.text = data["MessageText"].ToString();
        messageText.ChangeLangauge(SettingManager.Instance.language, data["TextKey"].ToString());
        
        //string[] buttonTexts = data["MessageButtonText"].ToString().Split('/');
        string[] buttonTexts = data["ButtonKey"].ToString().Split('/');
        select1.transform.parent.gameObject.SetActive(true);
        //select1.text = buttonTexts[0];
        select1.ChangeLangauge(SettingManager.Instance.language, buttonTexts[0]);
        targetQuest = data["TargetQuest"].ToString().Split('/').ToList();
        if (buttonTexts.Length <= 1)
            select2.transform.parent.gameObject.SetActive(false);
        else if(buttonTexts.Length >= 2)
        {
            if (buttonTexts.Length == 2)
            {
                //select2.text = buttonTexts[1];
                select2.ChangeLangauge(SettingManager.Instance.language, buttonTexts[1]);
            }
            else
            {
                //select2.text = "다른 선택지를 고려한다.";
                select2.ChangeLangauge(SettingManager.Instance.language, "q_msg_next");
            }
            select2.transform.parent.gameObject.SetActive(true);
        }

        bool isMain = data["Type"].ToString() == "main";
        if (img != null)
            img.sprite = sprites[isMain ? 0 : Random.Range(1, 4)];
        select1.transform.parent.GetComponent<Image>().sprite = isMain? mainSelectSprite[0] : subSelectSprite[0];
        select2.transform.parent.GetComponent<Image>().sprite = isMain ? mainSelectSprite[0] : subSelectSprite[0];
        select1Back.sprite = isMain ? mainSelectSprite[1] : subSelectSprite[1];
        select2Back.sprite = isMain ? mainSelectSprite[1] : subSelectSprite[1];

        select1.transform.parent.GetComponent<Button>().interactable = false;
        select2.transform.parent.GetComponent<Button>().interactable = false;

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f), false, default, cancellationTokenSource.Token);
        //string openClip = "Quest_Paper_Open_" + Random.Range(1, 4).ToString();
        //AudioManager.Instance.Play2DSound(openClip, SettingManager.Instance._FxVolume);
        FMODUnity.RuntimeManager.PlayOneShot(openSound);

        await UniTask.Delay(System.TimeSpan.FromSeconds(dissolveController.appareGoaltime), false, default, cancellationTokenSource.Token);
        select1.transform.parent.GetComponent<Button>().interactable = true;
        select2.transform.parent.GetComponent<Button>().interactable = true;
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
