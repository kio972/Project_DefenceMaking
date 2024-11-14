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

    private TooltipObject curTarget;

    public bool IsCheckValid
    {
        get
        {
            if (GameManager.Instance.tileLock)
                return false;
            if (InputManager.Instance.settingCard)
                return false;

            return true;
        }
    }

    private void SpawnerKeyInject(TooltipObject target)
    {
        MonsterSpawner spawner = target.GetComponentInParent<MonsterSpawner>();
        if(spawner == null) return;
        target.toolTipKey_header = "tooltip_spawner_" + spawner._TargetKey + "_0";
        target.toolTipKey_descs = "tooltip_spawner_" + spawner._TargetKey + "_1";
    }

    private void ShowIndex(TooltipObject target)
    {
        if (target.toolTipType == ToolTipType.Spawner)
            SpawnerKeyInject(target);
        

        TileControlUI tileControl = MonoBehaviour.FindObjectOfType<TileControlUI>();
        tileControl?.SetButton(target);

        IToolTipEffect curEffect = target.GetComponent<IToolTipEffect>();
        curEffect?.ShowEffect(true);
    }

    public void ForceReset()
    {
        if (curTarget != null)
        {
            IToolTipEffect prevEffect = curTarget.GetComponent<IToolTipEffect>();
            prevEffect?.ShowEffect(false);
        }

        prevTarget = null;
        curIndex = 0;
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
            //if (tooltipObject.toolTipType == ToolTipType.Tile)
            //{
            //    TileNode tileNode = tooltipObject.GetComponentInParent<TileNode>();
            //    if(tileNode == null || tileNode.curTile == null)
            //        continue;
            //}

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

            if (curTarget != null)
            {
                IToolTipEffect prevEffect = curTarget.GetComponent<IToolTipEffect>();
                prevEffect?.ShowEffect(false);
            }

            List<TooltipObject> tooltipObjects = GetTooltipObjects();

            if (tooltipObjects.Count <= 0 || prevTarget != tooltipObjects[0])
                ResetInput();

            if (tooltipObjects.Count <= 0)
            {
                TileControlUI tileControl = MonoBehaviour.FindObjectOfType<TileControlUI>();
                tileControl?.CloseAll();
                return;
            }

            if(prevTarget != null && prevTarget.toolTipType == ToolTipType.Devil && tooltipObjects[0].toolTipType == ToolTipType.Devil && !GameManager.Instance.moveLock)
            {
                TileControlUI tileControl = MonoBehaviour.FindObjectOfType<TileControlUI>();
                tileControl?.MoveTile(NodeManager.Instance.endPoint.curTile);
                return;
            }

            if (curIndex >= tooltipObjects.Count)
                curIndex = 0;
            curTarget = tooltipObjects[curIndex];
            ShowIndex(curTarget);

            prevTarget = tooltipObjects[0];
            curIndex++;
        }
        else if(Input.GetKeyDown(SettingManager.Instance.key_CancelControl._CurKey))
        {
            if (curTarget == null)
                return;

            IToolTipEffect prevEffect = curTarget.GetComponent<IToolTipEffect>();
            prevEffect?.ShowEffect(false);
            ResetInput();
        }
    }
}
