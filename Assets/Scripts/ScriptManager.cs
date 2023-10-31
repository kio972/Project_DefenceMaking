using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    private static ScriptManager instance;

    public static ScriptManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScriptManager>();
                if (instance == null)
                {
                    ScriptManager temp = Resources.Load<ScriptManager>("UI/ScriptManager");
                    temp = Instantiate(temp);
                    temp.name = typeof(ScriptManager).Name;
                    instance = temp;
                    instance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                }
            }

            return instance;
        }
    }

    private char choiceNum = 'a';

    private Queue<List<Dictionary<string, object>>> scriptQueue = new Queue<List<Dictionary<string, object>>>();

    public float textTime = 0.5f;

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

    public void MakeChoice(char choice)
    {
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
        // ÇÑ±ÛÀÚ¾¿ ±ÛÀÚ Ãâ·Â
        StringBuilder sb = new StringBuilder();
        WaitForSeconds waitTime = new WaitForSeconds(textTime);
        for (int i = 0; i < conver.Length; i++)
        {
            sb.Append(conver[i].ToString());
            targetText.text = sb.ToString();

            if (Input.GetKey(KeyCode.Mouse0))
            {
                targetText.text = conver;
                yield break;
            }

            yield return waitTime;
        }
    }

    private IEnumerator ScriptManage()
    {
        Vector2 closedPos1 = fadeUp.anchoredPosition;
        Vector2 closedPos2 = fadeDown.anchoredPosition;
        Vector2 openedPos1 = closedPos1 + new Vector2(0, 500);
        Vector2 openedPos2 = closedPos2 + new Vector2(0, -500);
        float lerpTime = 0.2f;
        
        while (true)
        {
            if (scriptQueue.Count == 0) { yield return null; continue; }

            // ÄÆÀÎ
            fadeUp.gameObject.SetActive(true);
            fadeDown.gameObject.SetActive(true);
            GameManager.Instance.speedController.SetSpeedZero();
            float elapsed = 0;
            while (elapsed < lerpTime)
            {
                fadeUp.anchoredPosition = Vector2.Lerp(openedPos1, closedPos1, elapsed / lerpTime);
                fadeDown.anchoredPosition = Vector2.Lerp(openedPos2, closedPos2, elapsed / lerpTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            List<Dictionary<string, object>> scripts = scriptQueue.Dequeue();
            foreach(Dictionary<string, object> script in scripts)
            {
                string type = script["type"].ToString();
                string conver = script["script"].ToString();
                string number = script["number"].ToString();

                if (type == "line")
                {
                    while (choiceState)
                        yield return null;

                    if (choiceNum == number[0])
                        continue;

                    yield return StartCoroutine("PrintScript", conver);

                    while (!Input.anyKeyDown)
                        yield return null;
                }
                else if (type == "choice")
                {
                    if (!choiceState)
                    {
                        ResetChoices();
                        choiceState = true;
                    }

                    SetChoice(conver, number[number.Length - 1]);
                }
            }

            // ÄÆ¾Æ¿ô
            elapsed = 0;
            while (elapsed < lerpTime)
            {
                fadeUp.anchoredPosition = Vector2.Lerp(closedPos1, openedPos1, elapsed / lerpTime);
                fadeDown.anchoredPosition = Vector2.Lerp(closedPos2, openedPos2, elapsed / lerpTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            GameManager.Instance.speedController.SetSpeedPrev(false);
            fadeUp.gameObject.SetActive(false);
            fadeDown.gameObject.SetActive(false);
            yield return null;
        }
    }

    public void EnqueueScript(string scriptID)
    {
        List<Dictionary<string, object>> scripts = DataManager.Instance.GetScripts(scriptID);
        scriptQueue.Enqueue(scripts);
    }

    private void Init()
    {
        StartCoroutine(ScriptManage());
        choiceTexts = GetComponentsInChildren<ChoiceText>(true).ToList();
        fadeUp.gameObject.SetActive(false);
        fadeDown.gameObject.SetActive(false);
    }
}
