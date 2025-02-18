using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetMesseage(Transform follow, Vector2 pivot, string header, string desc, string additional = null)
    {
        rect.pivot = pivot;
        followTarget = follow;
        transform.position = Camera.main.WorldToScreenPoint(followTarget.position);

        this.header.text = header;
        this.desc.text = desc;
        if(additional == null)
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
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position);
    }
}
