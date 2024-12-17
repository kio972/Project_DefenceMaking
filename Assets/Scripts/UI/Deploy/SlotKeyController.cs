using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ISlot
{
    void SendInfo();
}

public interface ISlotInformer
{
    ISlot curSlot { get; }
    void ExcuteAction();
}

public class SlotKeyController : MonoBehaviour, IKeyControl
{
    [SerializeField]
    private GridLayoutGroup slotGrid;
    [SerializeField]
    private Transform informerTranform;
    private ISlotInformer _informer;
    private ISlotInformer informer { get { if (_informer == null) _informer = informerTranform.GetComponent<ISlotInformer>(); return _informer; } }

    private BiMap<ISlot, (int row, int col)> biMap;
    private int maxRow;
    private int maxCol;

    private int curRow;
    private int curCol;

    private ISlot _curSlot;

    private void SelectSlot(int row, int col)
    {
        ISlot nextSlot = biMap.GetKey((row, col));
        if (nextSlot == null)
            return;

        curRow = row;
        curCol = col;
        _curSlot = nextSlot;
        _curSlot.SendInfo();
    }

    private void ForceUpdateSlot(ISlot targetSlot)
    {
        if (!biMap.IsValiedKey(targetSlot))
            return;
        var targetIndex = biMap.GetValue(targetSlot);
        curRow = targetIndex.row;
        curCol = targetIndex.col;
        _curSlot = targetSlot;
        _curSlot.SendInfo();
    }

    public void HandleInput(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.W:
                SelectSlot(curRow - 1, curCol);
                break;
            case KeyCode.A:
                SelectSlot(curRow, curCol - 1);
                break;
            case KeyCode.S:
                SelectSlot(curRow + 1, curCol);
                break;
            case KeyCode.D:
                SelectSlot(curRow, curCol + 1);
                break;
            case KeyCode.Space:
                informer?.ExcuteAction();
                break;
        }
    }

    private void Update()
    {
        if (informer == null || informer.curSlot == _curSlot)
            return;

        ForceUpdateSlot(informer.curSlot);
    }

    private void OnEnable()
    {
        biMap = new BiMap<ISlot, (int row, int col)>();
        int constraintCount = slotGrid.constraintCount;
        ISlot[] slots = slotGrid.GetComponentsInChildren<ISlot>();
        for(int i = 0; i < slots.Length; i++)
        {
            int row = i / constraintCount;
            int col = i % constraintCount;
            biMap.Add(slots[i], (row, col));
        }
        maxRow = (slots.Length - 1) / constraintCount;
        maxCol = constraintCount - 1;
        slots[0].SendInfo();

        curRow = biMap.GetValue(slots[0]).row;
        curCol = biMap.GetValue(slots[0]).col;
    }

    void OnGUI()
    {
        Event e = Event.current;

        // 키 입력 이벤트인지 확인
        if (e.isKey && e.type == EventType.KeyDown && e.keyCode != KeyCode.None)
        {
            if (!slotGrid.gameObject.activeInHierarchy)
                return;

            HandleInput(e.keyCode);
        }
    }
}
