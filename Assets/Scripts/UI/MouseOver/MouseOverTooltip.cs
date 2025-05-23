using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverTooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI header;
    [SerializeField]
    private TextMeshProUGUI desc;
    [SerializeField]
    private TextMeshProUGUI additional;

    [SerializeField]
    private RectTransform rect;

    private Transform followTarget = null;

    private bool isOnUI;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if(value)
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void SetMesseage(Vector3 targetPos, Vector2 pivot, bool isOnUI, string header, string desc, string additional = null)
    {
        rect.pivot = pivot;
        followTarget = null;
        this.isOnUI = isOnUI;
        transform.position = isOnUI ? targetPos : Camera.main.WorldToScreenPoint(targetPos);

        this.header.text = header;
        this.desc.text = desc;
        if (string.IsNullOrEmpty(additional))
        {
            this.additional.gameObject.SetActive(false);
        }
        else
        {
            this.additional.gameObject.SetActive(true);
            this.additional.text = additional;
        }
    }

    public void SetMesseage(Transform follow, Vector2 pivot, bool isOnUI, string header, string desc, string additional = null)
    {
        rect.pivot = pivot;
        followTarget = follow;
        this.isOnUI = isOnUI;
        transform.position = isOnUI ? followTarget.position : Camera.main.WorldToScreenPoint(followTarget.position);

        this.header.text = header;
        this.desc.text = desc;
        if(string.IsNullOrEmpty(additional))
        {
            this.additional.gameObject.SetActive(false);
        }
        else
        {
            this.additional.gameObject.SetActive(true);
            this.additional.text = additional;
        }
    }

    private void Update()
    {
        if(followTarget != null)
            transform.position = isOnUI ? followTarget.position : Camera.main.WorldToScreenPoint(followTarget.position);
    }
}
