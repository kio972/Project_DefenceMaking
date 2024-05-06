using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationControl : MonoBehaviour
{
    NotificationSlot[] slots;

    public float spacing = 25;

    private Vector2[] slotPos;

    private struct NotiQueue
    {
        public string msg;
        public NotificationType type;
        public NotiQueue(string msg, NotificationType type)
        {
            this.msg = msg;
            this.type = type;
        }
    }

    private Queue<NotiQueue> notiQueue;

    public float waitTime = 1f;
    private WaitForSeconds wait;

    private NotificationSlot _NextSlot
    {
        get
        {
            foreach(NotificationSlot slot in slots)
            {
                if (slot._CurState == NotificationState.Closed)
                    return slot;
            }
            return null;
        }
    }

    private List<NotificationSlot> activeSlot = new List<NotificationSlot>();

    private void ArrangeIndex(int index)
    {
        for(int i = index; i < activeSlot.Count; i++)
        {
            activeSlot[i].index -= 1;
        }

        activeSlot.RemoveAt(index);
    }

    private void PushSlot()
    {
        foreach(NotificationSlot slot in slots)
        {
            if (slot.index == -1)
                continue;
            slot.index -= 1;
            if (slot.index == -1)
            {
                slot.OnClick();
                activeSlot.Remove(slot);
            }
        }
    }

    public void SetMesseage(string msg, NotificationType type)
    {
        notiQueue.Enqueue(new NotiQueue(msg, type));
    }

    private void ModifyPos()
    {
        foreach(NotificationSlot slot in slots)
        {
            if (slot.index == -1 || slot._Rect.anchoredPosition == slotPos[slot.index])
                continue;

            slot.transform.Translate(Vector2.up * Time.deltaTime * 100);
            if (slot._Rect.anchoredPosition.y >= slotPos[slot.index].y)
                slot._Rect.anchoredPosition = slotPos[slot.index];
        }
    }

    private IEnumerator ManageNotification()
    {
        while(true)
        {
            if (notiQueue.Count == 0)
            {
                yield return null;
                continue;
            }

            NotificationSlot slot = _NextSlot;
            if (slot == null)
            {
                activeSlot[0].OnClick();
                //PushSlot();
                yield return wait;
                continue;
            }

            activeSlot.Add(slot);
            NotiQueue queue = notiQueue.Dequeue();
            slot.SetMesseage(queue.msg, queue.type);
            slot.index = activeSlot.Count - 1;

            yield return null;
        }
    }

    private void Awake()
    {
        slots = GetComponentsInChildren<NotificationSlot>(true);
        if (slots == null || slots.Length == 0)
            return;

        wait = new WaitForSeconds(waitTime);

        slotPos = new Vector2[slots.Length];
        notiQueue = new Queue<NotiQueue>();

        float pos = slots[0]._Rect.anchoredPosition.y;
        float width = slots[0]._Rect.sizeDelta.y;
        slotPos[0] = slots[0]._Rect.anchoredPosition;
        for (int i = 1; i < slots.Length; i++)
        {
            slots[i]._Rect.anchoredPosition = new Vector2(slots[i]._Rect.anchoredPosition.x, pos - width - spacing);
            pos = slots[i]._Rect.anchoredPosition.y;
            width = slots[i]._Rect.sizeDelta.y;
            slotPos[i] = slots[i]._Rect.anchoredPosition;
        }

        StartCoroutine(ManageNotification());
    }

    private void Update()
    {
        ModifyPos();
    }
}
