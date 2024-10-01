using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput
{
    void CheckInput();
}

public class TooltipInput : IInput
{
    HashSet<int> targetLayers = null;

    private GameObject prevTarget = null;
    private int curIndex = 0;

    private void ShowIndex(GameObject target)
    {
        
    }

    private void ResetInput()
    {
        prevTarget = null;
        curIndex = 0;
    }

    public void CheckInput()
    {
        if(Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey))
        {
            if(targetLayers == null)
                targetLayers = new HashSet<int>() { LayerMask.NameToLayer("CharacterModel"), LayerMask.NameToLayer("Terrain") };

            List<GameObject> targets = UtilHelper.RayCastLayer(targetLayers);
            if (targets == null || targets.Count <= 0 || prevTarget != targets[0])
                ResetInput();

            if (targets == null || targets.Count <= 0)
                return;

            prevTarget = targets[0];
            if (curIndex >= targets.Count)
                curIndex = 0;
            ShowIndex(targets[curIndex]);
            curIndex++;
        }
    }
}
