using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    private static StoryManager instance;

    public static StoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoryManager>();
                if (instance == null)
                {
                    StoryManager temp = Resources.Load<StoryManager>("Prefab/UI/StoryManager");
                    temp = Instantiate(temp);
                    temp.name = typeof(StoryManager).Name;
                    instance = temp;
                    instance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                }
            }

            return instance;
        }
    }

    private char choiceNum = ' ';

    private Queue<List<Dictionary<string, object>>> scriptQueue = new Queue<List<Dictionary<string, object>>>();
    public bool IsScriptQueueEmpty { get { if (scriptQueue.Count == 0) return true; else return false; } }

    private float textTime = 0.05f;

    [SerializeField]
    private TextMeshProUGUI targetText;

    private List<ChoiceText> choiceTexts;

    private bool choiceState = false;

    [SerializeField]
    private RectTransform fadeUp;
    [SerializeField]
    private RectTransform fadeDown;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private GameObject blocker;

    [SerializeField]
    private IllustrateUI leftIllust;
    [SerializeField]
    private IllustrateUI rightIllust;
    [SerializeField]
    private IllustrateUI middleIllust;

    private IllustrateUI prevIllust;

    public bool isSkip = false;

    [SerializeField]
    private GameObject nextArrow;

    [SerializeField]
    private Transform imgGroup;

    private Dictionary<string, IllustrateUI> illustDic;

    [SerializeField]
    private Button skipBtn;

    public MonoBehaviour triggerZone;

    private HashSet<IllustrateUI> curUsedIllur;

    private readonly Color fadedColor = new Color(0.4f, 0.4f, 0.4f);

    public void SkipScript()
    {
        if(!isSkip)
            isSkip = true;
    }

    public void MakeChoice(char choice)
    {
        targetText.text = "";
        choiceNum = choice;
        choiceState = false;
        ResetChoices();
    }

    private void ResetChoices()
    {
        foreach(ChoiceText text in choiceTexts)
            text.gameObject.SetActive(false);
    }

    private void SetChoice(string script, char choice)
    {
        foreach (ChoiceText text in choiceTexts)
        {
            if (text.gameObject.activeSelf)
                continue;

            text.SetChoice(script, choice);
            text.gameObject.SetActive(true);
            return;
        }

        ChoiceText newText = Instantiate(choiceTexts[0], choiceTexts[0].transform.parent);
        choiceTexts.Add(newText);
        newText.SetChoice(script, choice);
        newText.gameObject.SetActive(true);
    }

    private IEnumerator PrintScript(string conver)
    {
        yield return null;
        // �ѱ��ھ� ���� ���
        StringBuilder sb = new StringBuilder();
        float elapsed = 0f;
        bool skip = false;
        for (int i = 0; i < conver.Length; i++)
        {
            sb.Append(conver[i].ToString());
            if (conver[i] is ' ' or '\r' or '\n')
                continue;

            if(conver[i] is '<')
            {
                skip = true;
                continue;
            }

            if (skip)
            {
                if (conver[i] is '>')
                    skip = false;
                continue;
            }

            targetText.text = sb.ToString();

            elapsed = 0f;
            while (elapsed < textTime)
            {
                elapsed += Time.deltaTime;
                if (isSkip)
                    yield break;
                else if (Input.anyKeyDown && !Input.GetKey(KeyCode.Escape))
                {
                    targetText.text = conver;
                    yield return null;
                    yield break;
                }
                yield return null;
            }
        }
    }

    private void ResetIllust()
    {
        prevIllust = null;
        foreach (var illur in curUsedIllur)
            illur.FadeOut();
        //if (leftIllust.gameObject.activeSelf)
        //    leftIllust.FadeOut();
        //if (rightIllust.gameObject.activeSelf)
        //    rightIllust.FadeOut();
        //if(middleIllust.gameObject.activeSelf)
        //    middleIllust.FadeOut();
    }

    private IllustrateUI GetIllust(string targetName)
    {
        if(illustDic == null)
        {
            illustDic = new Dictionary<string, IllustrateUI>();
            IllustrateUI[] illustrates = imgGroup.GetComponentsInChildren<IllustrateUI>(true);
            foreach(var illustrate in illustrates)
            {
                illustDic[illustrate.name] = illustrate;
            }
        }

        if (illustDic.ContainsKey(targetName))
            return illustDic[targetName];
        else
            return null;
    }

    private void SetIllust(string targetName, bool isRight, float illurXPos, string track0, string track1)
    {
        IllustrateUI targetUI = GetIllust(targetName);
        if (targetUI == null)
        {
            prevIllust?.ChangeColor(fadedColor);
            prevIllust = null;
            return;
        }

        targetUI.SetRotation(isRight);
        targetUI.SetPosition(illurXPos);
        if (prevIllust != targetUI)
        {
            prevIllust?.ChangeColor(fadedColor);

            if (!targetUI.gameObject.activeSelf)
                targetUI.FadeIn();
            else
                targetUI.ChangeColor(Color.white);

            prevIllust = targetUI;
            curUsedIllur.Add(targetUI);
        }

        if (!string.IsNullOrEmpty(track0))
        {
            bool loop = track0 == "idle" ? true : false;
            string idle = track0 == "idle" ? "" : "idle";
            targetUI.SetAnim(track0, loop, 0, idle);
        }
        if (!string.IsNullOrEmpty(track1))
            targetUI.SetAnim(track1, false, 1);
    }

    private void SetIllust(string position, string track0, string track1)
    {
        IllustrateUI targetUI = null;
        if (position == "left")
            targetUI = leftIllust;
        else if (position == "right")
            targetUI = rightIllust;
        else if (position == "middle")
            targetUI = middleIllust;

        if (targetUI == null)
            return;

        if(prevIllust != targetUI)
        {
            prevIllust?.ChangeColor(fadedColor);

            if (!targetUI.gameObject.activeSelf)
                targetUI.FadeIn();
            else
                targetUI.ChangeColor(Color.white);

            prevIllust = targetUI;
        }
        
        if (track0 != "")
        {
            bool loop = track0 == "idle" ? true : false;
            string idle = track0 == "idle" ? "" : "idle";
            targetUI.SetAnim(track0, loop, 0, idle);
        }
        if (track1 != "")
            targetUI.SetAnim(track1, false, 1);
    }

    private void StopTalkAnim()
    {
        prevIllust?.StopTalkAnimation();
    }

    private void PlayTalkAnim(int length)
    {
        float talkTime = 3;
        if (length < 30)
            talkTime = 2;
        else if (length < 10)
            talkTime = 1;
        prevIllust?.PlayTalkAnimation(talkTime);
    }

    private IEnumerator ScriptManage()
    {
        Vector2 closedPos1 = fadeUp.anchoredPosition;
        Vector2 closedPos2 = fadeDown.anchoredPosition;
        Vector2 openedPos1 = closedPos1 + new Vector2(0, fadeUp.sizeDelta.y);
        Vector2 openedPos2 = closedPos2 + new Vector2(0, fadeDown.sizeDelta.y * -1f);
        float lerpTime = 0.5f;

        while (true)
        {
            if (scriptQueue.Count == 0) { yield return null; continue; }
            nameText.text = "";
            targetText.text = "";
            curUsedIllur = new HashSet<IllustrateUI>();
            // ����
            fadeUp.gameObject.SetActive(true);
            fadeDown.gameObject.SetActive(true);
            blocker.gameObject.SetActive(true);
            GameManager.Instance.SetPause(true);
            skipBtn.gameObject.SetActive(false);
            float elapsed = 0;
            while (elapsed < lerpTime)
            {
                fadeUp.anchoredPosition = Vector2.Lerp(openedPos1, closedPos1, elapsed / lerpTime);
                fadeDown.anchoredPosition = Vector2.Lerp(openedPos2, closedPos2, elapsed / lerpTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            fadeUp.anchoredPosition = closedPos1;
            fadeDown.anchoredPosition = closedPos2;
            skipBtn.gameObject.SetActive(true);
            List<Dictionary<string, object>> scripts = scriptQueue.Peek();
            foreach(Dictionary<string, object> script in scripts)
            {
                if (isSkip)
                    break;

                string type = script["type"].ToString();
                string conver = script[SettingManager.Instance.language.ToString()].ToString();
                string number = script["number"].ToString();
                string name = DataManager.Instance.GetDescription(script["nameKey"].ToString());
                string illustName = script["character sprite"].ToString();
                string illustPos = script["sprite position"].ToString();
                float illurXPos = 0;
                float.TryParse(script["sprite position"].ToString(), out illurXPos);
                bool isRight = script["direction"].ToString() == "right" ? true : false;
                string trigger = script["trigger"].ToString();
                string track0 = script["track0"].ToString();
                string track1 = script["track1"].ToString();
                string act = script["act"].ToString();

                if (type == "line")
                {
                    while (choiceState)
                    {
                        if (isSkip)
                            break;
                        yield return null;
                    }

                    if (isSkip)
                        break;

                    if (number[1] != '_')
                        choiceNum = ' ';

                    if (choiceNum != ' ' && choiceNum != number[0])
                        continue;

                    StopTalkAnim();
                    SetIllust(illustName, isRight, illurXPos, track0, track1);
                    
                    if (trigger != "")
                        SendMessage(trigger, SendMessageOptions.DontRequireReceiver);

                    if (nameText.text != name)
                        nameText.text = name;

                    //SetIllust(illustPos, track0, track1);

                    if (conver != "")
                    {
                        if(act != "NoTalk")
                            PlayTalkAnim(conver.Length);

                        yield return StartCoroutine(PrintScript(conver));
                        //targetText.text = conver;
                        //yield return null;
                        if (isSkip)
                            break;
                        if (act != "NoWait")
                        {
                            nextArrow.SetActive(true);
                            while (!Input.anyKeyDown || Input.GetKey(KeyCode.Escape))
                                yield return null;
                            nextArrow.SetActive(false);
                        }
                    }
                }
                else if (type == "choice")
                {
                    if (!choiceState)
                    {
                        ResetChoices();
                        choiceState = true;
                    }
                    if (trigger != "")
                        SendMessage(trigger, SendMessageOptions.DontRequireReceiver);
                    SetChoice(conver, number[number.Length - 1]);
                }
            }

            // �ƾƿ�
            ResetIllust();
            elapsed = 0;
            while (elapsed < lerpTime)
            {
                fadeUp.anchoredPosition = Vector2.Lerp(closedPos1, openedPos1, elapsed / lerpTime);
                fadeDown.anchoredPosition = Vector2.Lerp(closedPos2, openedPos2, elapsed / lerpTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            isSkip = false;
            choiceState = false;
            ResetChoices();
            fadeUp.anchoredPosition = openedPos1;
            fadeDown.anchoredPosition = openedPos2;

            GameManager.Instance.SetPause(false);
            fadeUp.gameObject.SetActive(false);
            fadeDown.gameObject.SetActive(false);
            blocker.gameObject.SetActive(false);

            scriptQueue.Dequeue();
            foreach(var illur in curUsedIllur)
                illur.SetColor(Color.white);
            //leftIllust.SetColor(Color.white);
            //rightIllust.SetColor(Color.white);
            //middleIllust.SetColor(Color.white);

            yield return null;
        }
    }

    #region TriggerZone

    private void DeActiveSkip()
    {
        skipBtn.gameObject.SetActive(false);
    }

    private void SetBuff()
    {
        foreach (ChoiceText choice in choiceTexts)
            choice.setInfo = true;
    }

    private void AddBuff()
    {
        char choice = choiceNum;
        PassiveManager.Instance.AddBuffTable(DataManager.Instance.BuffTable[GameManager.Instance.loop][choice]);
    }

    private void ResetBuff()
    {
        foreach (ChoiceText choice in choiceTexts)
            choice.setInfo = false;
    }

    private void SetBlack()
    {
        prevIllust?.SetColor(Color.black);
    }

    private void FadeBlack()
    {
        if (prevIllust == null)
            return;
        prevIllust.ChangeColor(Color.black, 0.5f);
    }

    private void FadeIn()
    {
        if (prevIllust == null)
            return;
        prevIllust.fadeInState = true;
        prevIllust.ChangeColor(Color.white);
    }

    private void FadeInLeft()
    {
        leftIllust.fadeInState = true;
        leftIllust.ChangeColor(Color.white);
    }

    private void FadeInRight()
    {
        rightIllust.fadeInState = true;
        rightIllust.ChangeColor(Color.white);
    }

    private void SetBlackLeft()
    {
        leftIllust.SetColor(Color.black);
    }

    private void SetBlackRight()
    {
        rightIllust.SetColor(Color.black);
    }

    private void ZoomStartTile()
    {
        GameManager.Instance.cameraController.ResetCamPos(true);
        GameManager.Instance.cameraController.SetCamZoom(4);
    }

    private void ZoomEndTile()
    {
        GameManager.Instance.cameraController.ResetCamPos();
        GameManager.Instance.cameraController.SetCamZoom(4);
    }

    private void ResetCamPos()
    {
        GameManager.Instance.cameraController.CamMoveToPos(Vector3.right * 2.5f);
        GameManager.Instance.cameraController.SetCamZoom(1);
    }

    private void SkipTutorial()
    {
        SceneController.Instance.MoveScene("Stage1", 0f);
    }

    #endregion

    public void EnqueueScript(string scriptID)
    {
        List<Dictionary<string, object>> scripts = DataManager.Instance.GetScripts(scriptID);
        if (scripts == null)
            return;
        scriptQueue.Enqueue(scripts);
    }

    private void Init()
    {
        StartCoroutine(ScriptManage());
        choiceTexts = GetComponentsInChildren<ChoiceText>(true).ToList();
        fadeUp.gameObject.SetActive(false);
        fadeDown.gameObject.SetActive(false);
        nameText.text = "";
        targetText.text = "";
        blocker.gameObject.SetActive(false);
        triggerZone = this;
    }
}
