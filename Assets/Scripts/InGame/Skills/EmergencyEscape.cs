using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    public float coolRate { get; }
    public bool isReady { get; }
    public bool UseSkill();
}

public class EmergencyEscape : ISkill
{
    private float coolTime = 4320f;
    private float nextTime = 0;

    public float coolRate
    {
        get
        {
            if (GameManager.Instance.TotalTime >= nextTime)
                return 0;
            else
                return (nextTime - GameManager.Instance.TotalTime) / coolTime;
        }
    }

    public bool isReady
    {
        get
        {
            if (GameManager.Instance.king.curHp < 5)
                return false;

            if (GameManager.Instance.TotalTime < nextTime)
                return false;

            return true;
        }
    }

    public bool UseSkill()
    {
        var nodes = NodeManager.Instance.GetEndTileMovableNodes();
        nodes.Remove(NodeManager.Instance.endPoint);

        if(nodes.Count <= 0)
            return false;

        int randomPoint = Random.Range(0, nodes.Count);

        var kingTile = NodeManager.Instance.endPoint.curTile;
        var nextNode = nodes[randomPoint];

        kingTile.MoveTile(nextNode);
        if (kingTile is TileEnd endTile)
            endTile.ForceRotate();
        NodeManager.Instance.SetGuideState(GuideState.None);

        CameraController cameraController = MonoBehaviour.FindObjectOfType<CameraController>();
        if (cameraController != null)
            cameraController.CamMoveToPos(kingTile.transform.position);

        nextTime = GameManager.Instance.TotalTime + coolTime;
        return true;
    }
}
