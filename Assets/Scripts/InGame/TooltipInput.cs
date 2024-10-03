using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInput
{
    public bool IsCheckValid { get; }
    public void CheckInput();
}

public class TooltipInput : IInput
{
    private TooltipObject prevTarget = null;
    private int curIndex = 0;

    public bool IsCheckValid
    {
        get
        {
            if (GameManager.Instance.tileLock)
                return false;

            return true;
        }
    }

    private void ShowIndex(TooltipObject target)
    {
        Debug.Log(target.name);
    }

    private void ResetInput()
    {
        prevTarget = null;
        curIndex = 0;
    }

    private List<TooltipObject> GetTooltipObjects()
    {
        List<TooltipObject> targets = new List<TooltipObject>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray();
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            TooltipObject tooltipObject = hitObject.GetComponent<TooltipObject>();
            if (tooltipObject == null)
                continue;
            if (tooltipObject.toolTipType == ToolTipType.Tile)
            {
                TileNode tileNode = tooltipObject.GetComponentInParent<TileNode>();
                if(tileNode == null || tileNode.curTile == null)
                    continue;
            }

            targets.Add(tooltipObject);
        }

        return targets.OrderBy(x => x.toolTipType).ThenBy(y => y.subLevel).ToList();
    }

    public void CheckInput()
    {
        if(Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            List<TooltipObject> tooltipObjects = GetTooltipObjects();

            if (tooltipObjects.Count <= 0 || prevTarget != tooltipObjects[0])
                ResetInput();

            if (tooltipObjects.Count <= 0)
                return;

            prevTarget = tooltipObjects[0];
            if (curIndex >= tooltipObjects.Count)
                curIndex = 0;
            ShowIndex(tooltipObjects[curIndex]);
            curIndex++;
        }
    }
}
