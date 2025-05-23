using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EmergencyEscape : ISkill, IHaveCost
{
    public bool IsPassive { get => false; }

    private float coolTime = 4320f;
    private float curCoolTime = 0;

    private PlayerBattleMain king;

    public ReactiveProperty<float> coolRate { get; } = new ReactiveProperty<float>(0);

    public ReactiveProperty<bool> isReady { get; } = new ReactiveProperty<bool>(false);

    private int _cost = 5;
    public int cost { get => _cost; }
    public string costType { get => "Hp"; }

    public void ReduceHpCost(int value)
    {
        _cost -= value;
    }

    public void ReduceCoolTime(float value)
    {
        value = Mathf.Max(1 - value, 0);
        coolTime = coolTime * value;
    }

    private void KnockBackOthers(TileNode targetNode)
    {
        TileNode enterance = null;
        foreach(var direction in targetNode.neighborNodeDic.Keys)
        {
            TileNode neighbor = targetNode.neighborNodeDic[direction];
            if (neighbor.curTile != null && neighbor.curTile.PathDirection.Contains(UtilHelper.ReverseDirection(direction)))
            {
                enterance = neighbor;
                break;
            }
        }

        List<Battler> targets = new List<Battler>();
        foreach(var target in GameManager.Instance.adventurersList)
            if (target.curNode == targetNode)
                targets.Add(target);

        foreach (var target in GameManager.Instance.monsterList)
            if (target.curNode == targetNode)
                targets.Add(target);

        foreach(var target in targets)
            target.ForceKnockBack(king, enterance, enterance.transform.position, 30);
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

        KnockBackOthers(NodeManager.Instance.endPoint);

        kingTile.AutoRotate(nextNode);
        kingTile.MoveTile(nextNode);

        NodeManager.Instance.SetGuideState(GuideState.None);

        CameraController cameraController = MonoBehaviour.FindObjectOfType<CameraController>();
        if (cameraController != null)
            cameraController.CamMoveToPos(kingTile.transform.position);

        king.MoveToBossRoom();
        if (_cost > 0)
            king.ForceGetDamage(_cost);
        curCoolTime = coolTime;
        return true;
    }

    public void SkillInit()
    {
        king = GameManager.Instance.king;
        var hpStream = Observable.EveryLateUpdate().Where(_ => king.curHp <= _cost).Subscribe(_ => { isReady.Value = false; }).AddTo(king.gameObject);
        var hpStream2 = Observable.EveryLateUpdate().Where(_ => king.curHp > _cost && coolRate.Value == 0).Subscribe(_ => { isReady.Value = true; }).AddTo(king.gameObject);

        var coolTimeStream = Observable.EveryUpdate().Where(_ => curCoolTime > 0)
            .Subscribe(_ =>
            {
                curCoolTime -= GameManager.Instance.InGameDeltaTime;
                if (curCoolTime < 0)
                    curCoolTime = 0;

                coolRate.Value = curCoolTime <= 0 ? 0 : curCoolTime / coolTime;
            }).AddTo(king.gameObject);
    }

    public SkillData SaveSkill()
    {
        SkillData skillData = new SkillData();
        skillData.skillName = GetType().Name;
        skillData.curCool = curCoolTime;
        return skillData;
    }

    public void LoadSkill(SkillData data)
    {
        curCoolTime += data.curCool;
    }
}
