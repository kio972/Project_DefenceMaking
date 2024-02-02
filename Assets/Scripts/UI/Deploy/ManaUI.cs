using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaUI : MonoBehaviour
{
    [SerializeField]
    protected Transform targetObject;

    [SerializeField]
    protected float xOffset = 0f;
    [SerializeField]
    protected float yOffset = 0f;

    private CompleteRoom targetRoom;

    [SerializeField]
    private TextMeshProUGUI text;

    public virtual void Init(Transform followTarget, CompleteRoom targetRoom)
    {
        this.targetObject = followTarget;
        this.targetRoom = targetRoom;
        text.text = targetRoom._RemainingMana.ToString();
    }

    protected virtual void Update()
    {
        if (targetObject == null || targetRoom == null)
            return;

        //transform.position = targetObject.transform.position;
        transform.position = Camera.main.WorldToScreenPoint(targetObject.transform.position) + new Vector3(xOffset, yOffset);
        text.text = targetRoom._RemainingMana.ToString();
    }
}
