using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EmergencyEscape : ISkill
{
    public bool IsPassive { get => false; }

    private float coolTime = 4320f;
    private float curCoolTime = 0;

    private Battler king;

    public ReactiveProperty<float> coolRate { get; } = new ReactiveProperty<float>(0);

    public ReactiveProperty<bool> isReady { get; } = new ReactiveProperty<bool>(false);

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

        king.GetDamage(5, null);
        curCoolTime = coolTime;
        return true;
    }

    public void SkillInit()
    {
        king = GameManager.Instance.king;
        var hpStream = Observable.EveryLateUpdate().Where(_ => king.curHp < 5).Subscribe(_ => { isReady.Value = false; }).AddTo(king.gameObject);

        var coolTimeStream = Observable.EveryUpdate().Where(_ => curCoolTime > 0)
            .Subscribe(_ =>
            {
                curCoolTime -= GameManager.Instance.InGameDeltaTime;
                if (curCoolTime < 0)
                    curCoolTime = 0;

                coolRate.Value = curCoolTime <= 0 ? 0 : curCoolTime / coolTime;
            }).AddTo(king.gameObject);
    }
}
