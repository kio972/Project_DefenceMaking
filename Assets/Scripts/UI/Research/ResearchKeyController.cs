using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public interface IKeyControl
{
    void HandleInput(KeyCode key);
}

public class ResearchKeyController : MonoBehaviour, IKeyControl
{
    [SerializeField]
    private List<GameObject> researchPages = new List<GameObject>();
    [SerializeField]
    private ResearchPopup informer;

    private Dictionary<GameObject, ScrollRect> researchScrollDic;
    private Dictionary<GameObject, BiMap<ISlot, (int row, int col)>> researchBiMapDic;

    [SerializeField]
    private List<ResearchSelectBtn> researchSelectBtns;
    private int _curResearchSelectIndex = 0;
    private ResearchSelectBtn curSelectPage { get => researchSelectBtns[_curResearchSelectIndex]; }
    
    [SerializeField]
    private ResearchSelectControl researchSelectControl;

    private int _curRow;
    private int _curCol;
    private ISlot _curSlot;

    private List<ScrollViewController> _scrollControllers = new List<ScrollViewController>();
    private ScrollViewController curActiveScrollView
    {
        get
        {
            foreach(var controller in _scrollControllers)
            {
                if(controller.gameObject.activeInHierarchy)
                    return controller;
            }
            return null;
        }
    }

    private BiMap<ISlot, (int row, int col)> curActiveBiMap
    {
        get
        {
            foreach(var item in researchBiMapDic.Keys)
            {
                if (item.activeInHierarchy)
                    return researchBiMapDic[item];
            }
            return null;
        }
    }

    private (int row, int col) MoveToUp(int curRow, int curCol)
    {
        ISlot nextSlot;
        int nextRow = curRow - 1;
        int nextCol = curCol;
        if (nextRow < 0)
            return (-1, 0);

        nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        while (nextSlot is ResearchSlot researchSlot && !researchSlot.isActivedResearch)
        {
            nextRow--;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if (nextSlot == null)
            return (-1, -1);

        return (nextRow, nextCol);
    }

    private (int row, int col) MoveToDown(int curRow, int curCol)
    {
        ISlot nextSlot;
        int nextRow = curRow + 1;
        int nextCol = curCol;

        nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        while (nextSlot is ResearchSlot researchSlot && !researchSlot.isActivedResearch)
        {
            nextRow++;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if (nextSlot == null)
            return (-1, -1);

        return (nextRow, nextCol);
    }

    private (int row, int col) MoveToLeft(int curRow, int curCol)
    {
        int nextRow = curRow;
        int nextCol = curCol - 1;

        ISlot nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        if (nextSlot is ResearchSlot researchSlot && !researchSlot.isActivedResearch)
        {
            var nextIndex = MoveToUp(nextRow, nextCol);
            nextRow = nextIndex.row;
            nextCol = nextIndex.col;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if(nextSlot == null)
        {
            nextRow = curRow;
            nextCol = curCol - 1;
            var nextIndex = MoveToDown(nextRow, nextCol);
            nextRow = nextIndex.row;
            nextCol = nextIndex.col;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if (nextSlot == null)
            return (-1, -1);

        return (nextRow, nextCol);
    }

    private (int row, int col) MoveToRight(int curRow, int curCol)
    {
        int nextRow = curRow;
        int nextCol = curCol + 1;
        if (nextCol >= 6)
            return (-1, -1);

        ISlot nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        if (nextSlot is ResearchSlot researchSlot && !researchSlot.isActivedResearch)
        {
            var nextIndex = MoveToUp(nextRow, nextCol);
            nextRow = nextIndex.row;
            nextCol = nextIndex.col;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if (nextSlot == null)
        {
            nextRow = curRow;
            nextCol = curCol + 1;
            var nextIndex = MoveToDown(nextRow, nextCol);
            nextRow = nextIndex.row;
            nextCol = nextIndex.col;
            nextSlot = curActiveBiMap.GetKey((nextRow, nextCol));
        }

        if (nextSlot == null)
            return (-1, -1);

        return (nextRow, nextCol);
    }

    private void SelectSlot(int row, int col)
    {
        ISlot nextSlot = curActiveBiMap.GetKey((row, col));
        if (nextSlot == null)
            return;
        _curRow = row;
        _curCol = col;
        _curSlot = nextSlot;
        _curSlot.SendInfo();

        if(_curSlot is ResearchSlot researchSlot)
        {
            RectTransform target = researchSlot.GetComponent<RectTransform>();
            curActiveScrollView?.ScrollToContent(target);
        }
    }

    private bool isOnSelectZone = true;

    public void HandleInput(KeyCode key)
    {
        if(!isOnSelectZone)
        {
            (int row, int col) nextIndex;
            switch (key)
            {
                case KeyCode.W:
                    nextIndex = MoveToUp(_curRow, _curCol);
                    if (nextIndex.row == -1 && nextIndex.col == 0)
                    {
                        isOnSelectZone = true;
                        informer.ResetPopUp();
                        researchSelectBtns[_curResearchSelectIndex].OnClick();
                    }
                    else
                        SelectSlot(nextIndex.row, nextIndex.col);
                    break;
                case KeyCode.A:
                    nextIndex = MoveToLeft(_curRow, _curCol);
                    SelectSlot(nextIndex.row, nextIndex.col);
                    break;
                case KeyCode.S:
                    nextIndex = MoveToDown(_curRow, _curCol);
                    SelectSlot(nextIndex.row, nextIndex.col);
                    break;
                case KeyCode.D:
                    nextIndex = MoveToRight(_curRow, _curCol);
                    SelectSlot(nextIndex.row, nextIndex.col);
                    break;
                case KeyCode.Space:
                    informer.ResearchInteract();
                    break;
            }
        }
        else
        {
            switch (key)
            {
                case KeyCode.W:
                    break;
                case KeyCode.A:
                    _curResearchSelectIndex = Mathf.Max(0, _curResearchSelectIndex - 1);
                    researchSelectBtns[_curResearchSelectIndex].OnClick();
                    break;
                case KeyCode.S:
                    isOnSelectZone = false;
                    researchSelectBtns[_curResearchSelectIndex].SetBaseBtn();
                    break;
                case KeyCode.D:
                    _curResearchSelectIndex = Mathf.Min(researchSelectBtns.Count - 1, _curResearchSelectIndex + 1);
                    researchSelectBtns[_curResearchSelectIndex].OnClick();
                    break;
                case KeyCode.Space:
                    isOnSelectZone = false;
                    researchSelectBtns[_curResearchSelectIndex].SetBaseBtn();
                    break;
            }
        }
    }

    private void ForceUpdateSlot(ISlot targetSlot)
    {
        if (!curActiveBiMap.IsValiedKey(targetSlot))
            return;
        var targetIndex = curActiveBiMap.GetValue(targetSlot);
        _curRow = targetIndex.row;
        _curCol = targetIndex.col;
        _curSlot = targetSlot;
        _curSlot.SendInfo();
    }

    private void Start()
    {
        researchScrollDic = new Dictionary<GameObject, ScrollRect>();
        researchBiMapDic = new Dictionary<GameObject, BiMap<ISlot, (int row, int col)>>();

        foreach (var page in researchPages)
        {
            ScrollRect researchPage = page.GetComponentInChildren<ScrollRect>(true);
            researchScrollDic[page] = researchPage;
            ScrollViewController scrollViewController = page.GetComponent<ScrollViewController>();
            if(scrollViewController != null)
                _scrollControllers.Add(scrollViewController);

            GridLayoutGroup group = page.GetComponentInChildren<GridLayoutGroup>(true);
            int rowCount = group.constraintCount;
            BiMap<ISlot, (int row, int col)> biMap = new BiMap<ISlot, (int row, int col)>();
            ISlot[] slots = group.GetComponentsInChildren<ISlot>(true);
            int curRow = 0;
            int curCol = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (curRow == rowCount)
                {
                    curRow = 0;
                    curCol++;
                }

                biMap.Add(slots[i], (curRow, curCol));
                curRow++;
            }
            researchBiMapDic[group.gameObject] = biMap;
        }
    }

    private void Update()
    {
        CheckClickSlot();
        CheckClickPage();
    }

    private void CheckClickPage()
    {
        if (informer == null || informer.curSlot != null)
            return;

        if (curSelectPage == researchSelectControl.curBtn)
            return;

        for(int i = 0; i < researchSelectBtns.Count; i++)
        {
            if(researchSelectBtns[i] == researchSelectControl.curBtn)
            {
                _curResearchSelectIndex = i;
                _curRow = 0;
                _curCol = -1;
                isOnSelectZone = true;
                break;
            }
        }
    }

    private void CheckClickSlot()
    {
        if (informer == null || informer.curSlot == null || informer.curSlot == _curSlot)
            return;

        isOnSelectZone = false;
        ForceUpdateSlot(informer.curSlot);
    }

    void OnGUI()
    {
        Event e = Event.current;

        // 키 입력 이벤트인지 확인
        if (e.isKey && e.type == EventType.KeyDown && e.keyCode != KeyCode.None)
        {
            if (curActiveBiMap == null)
                return;

            HandleInput(e.keyCode);
        }
    }
}