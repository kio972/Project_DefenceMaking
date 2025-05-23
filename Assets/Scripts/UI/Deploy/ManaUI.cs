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

    public void Init(Transform followTarget, List<Tile> targetRoom, bool isCompleteRoom)
    {
        this.targetObject = followTarget;
        this.targetRoom = null;

        int totalMana = 0;
        foreach(var tile in targetRoom)
        {
            totalMana += tile.RoomMana;
        }
        text.text = totalMana.ToString();
        text.color = isCompleteRoom ? Color.white : Color.red;
    }

    public void Init(Transform followTarget, string desc)
    {
        this.targetObject = followTarget;
        this.targetRoom = null;
        text.text = desc;
    }

    protected virtual void Update()
    {
        if (targetObject == null)
            return;

        //transform.position = targetObject.transform.position;
        transform.position = Camera.main.WorldToScreenPoint(targetObject.transform.position) + new Vector3(xOffset, yOffset);
        if(targetRoom != null)
            text.text = targetRoom._RemainingMana.ToString();
    }
}
