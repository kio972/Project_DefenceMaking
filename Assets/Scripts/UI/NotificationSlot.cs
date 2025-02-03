using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum NotificationType
{
    Deploy,
    Research,
    Shop,
}

public enum NotificationState
{
    Open,
    Wait,
    Closed,
}

public class NotificationSlot : MonoBehaviour
{
    public int index = -1;

    private RectTransform rect;
    public RectTransform _Rect
    {
        get
        {
            if (rect == null)
                rect = transform.GetComponent<RectTransform>();
            return rect;
        }
    }

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Image img;

    [SerializeField]
    private Button btn;

    [SerializeField]
    Animator animator;

    [SerializeField]
    private TextMeshProUGUI text;

    private NotificationState curState = NotificationState.Closed;
    public NotificationState _CurState { get => curState; }

    [SerializeField]
    private GameObject linkedPage;

    public void Finish()
    {
        curState = NotificationState.Closed;
        gameObject.SetActive(false);
        index = -1;
    }

    private void Wait()
    {
        curState = NotificationState.Wait;
    }

    public void ForceUpdateText(string msg)
    {
        text.text = msg;
    }

    public void SetMesseage(string msg, NotificationType type)
    {
        animator.Rebind();
        text.text = msg;
        gameObject.SetActive(true);
        curState = NotificationState.Open;
        if(img != null && sprites != null)
            img.sprite = sprites[(int)type];
    }

    public void OnClick()
    {
        animator.SetTrigger("OnClick");
        //SendMessageUpwards("ArrangeIndex", index);
    }

    private void Awake()
    {
        btn.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        if(curState == NotificationState.Wait && linkedPage != null && linkedPage.activeInHierarchy)
            Finish();
    }

    private void Start()
    {
        LanguageManager.Instance.AddLanguageAction(() => OnClick());
    }
}
