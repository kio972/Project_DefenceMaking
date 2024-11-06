using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MonsterType
{
    none,
    slime,
    goblin,
    golem,
    mimic,
    undead,
}

public interface IHoldbacker
{
    HashSet<Battler> holdBackTargets { get; }
    bool CanHoldBack { get; }
    void AddHoldBackTarget(Battler target);
}

public class Monster : Battler, IHoldbacker
{
    private int monsterIndex = -1;

    [SerializeField]
    private bool isHide = false;
    [SerializeField]
    protected int holdBackCount = 1;

    protected int requiredMana;
    public int _RequiredMana { get => requiredMana; }

    MonsterType monsterType;

    private int resurrectCount;

    private HashSet<Battler> _holdBackTargets = new HashSet<Battler>();

    public HashSet<Battler> holdBackTargets { get => _holdBackTargets; }

    [SerializeField]
    private AudioClip summonSound;

    //public override float MoveSpeed
    //{
    //    get
    //    {
    //        float speed = base.MoveSpeed;
    //        if (curTile != null && curTile.curTile != null)
    //            speed *= (100f + PassiveManager.Instance._TileSpeed_Weight[(int)curTile.curTile._TileType]) / 100f;

    //        return speed;
    //    }
    //}

    public bool CanHoldBack
    {
        get
        {
            List<Battler> curHoldBackTargets = new List<Battler>(_holdBackTargets);
            foreach(var target in curHoldBackTargets)
                if(target.isDead)
                    _holdBackTargets.Remove(target);

            return _holdBackTargets.Count < holdBackCount;
        }
    }

    public void AddHoldBackTarget(Battler target)
    {
        _holdBackTargets.Add(target);
        GameManager.Instance.holdBackedABattlers.Add(target);
    }

    public override void Dead()
    {
        if(resurrectCount > 0)
        {
            resurrectCount--;
            curHp = maxHp;
            return;
        }

        base.Dead();
        GameManager.Instance.SetMonseter(this, false);
        //if (curTile.curTile.monster != null && curTile.curTile.monster == this)
        //{
        //    curTile.curTile.monster = null;
        //    if (NodeManager.Instance._GuideState == GuideState.Monster)
        //        NodeManager.Instance.SetGuideState(GuideState.Monster);
        //}
    }

    protected override void DirectPass(TileNode targetTile)
    {
        if (targetTile == null)
        {
            ResetPaths();
            return;
        }

        if (_nextNode == null || _nextNode.curTile == null)
        {
            List<TileNode> path = PathFinder.FindPath(_curNode, targetTile);
            if ((transform.position - _curNode.transform.position).magnitude > 0.001f && path.Count >= 2 && UtilHelper.ReverseDirection(path[1].GetNodeDirection(path[0])) == UtilHelper.CheckClosestDirection(transform.position - _curNode.transform.position))
                path.Remove(_curNode);

            if (path != null && path.Count > 0)
                _nextNode = path[0];
            else
            {
                ResetPaths();
                return;
            }
        }

        ExcuteMove(_nextNode);

        // nextNode까지 이동완료
        if (Vector3.Distance(transform.position, _nextNode.transform.position) < 0.001f)
            NodeAction(_nextNode);
    }

    protected override void CollapseLogic()
    {
        if (isCollapsed)
        {
            collapseCool += Time.deltaTime * GameManager.Instance.timeScale;
            if (collapseCool >= 1f)
            {
                collapseCool = 0f;
                GetDamage(Mathf.RoundToInt(maxHp * 0.05f), null);
            }
        }
    }

    public override void Patrol()
    {
        CollapseLogic();

        if (directPass)
            DirectPass(lastCrossRoad);
        else
            NormalPatrol();
    }

    protected override TileNode FindNextNode(TileNode curNode)
    {
        //startNode에서 roomDirection이나 pathDirection이 있는 방향의 이웃노드를 받아옴
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(_prevNode);
        nextNodes.Remove(NodeManager.Instance.startPoint);
        nextNodes.Remove(NodeManager.Instance.endPoint);
        if (crossedNodes != null)
        {
            foreach (TileNode node in crossedNodes)
                nextNodes.Remove(node);
        }

        if (nextNodes.Count == 0)
            return null;

        //갈림길일경우 저장
        if (nextNodes.Count > 1)
            lastCrossRoad = curNode;

        TileNode nextNode = nextNodes[UnityEngine.Random.Range(0, nextNodes.Count)];
        return nextNode;
    }

    public void SetStartPoint(TileNode tile)
    {
        transform.position = tile.transform.position;
        _curNode = tile;
    }

    private void ModifyPassive()
    {
        armor += PassiveManager.Instance.monsterDefense_Weight;
        attackSpeed *= ((100 + PassiveManager.Instance.monsterAttackSpeed_Weight) / 100);
        attackSpeed *= ((100 + PassiveManager.Instance._MonsterTypeAttackSpeed_Weight[(int)monsterType]) / 100);

        minDamage = (int)((float)minDamage * ((100 + PassiveManager.Instance.monsterDamageRate_Weight) / 100));
        maxDamage = (int)((float)maxDamage * ((100 + PassiveManager.Instance.monsterDamageRate_Weight) / 100));

        maxHp += PassiveManager.Instance._MonsterTypeHp_Weight[(int)monsterType];
        maxHp += PassiveManager.Instance.monsterHp_Weight;
        maxHp = Mathf.FloorToInt((float)maxHp * ((100 + PassiveManager.Instance.monsterHpRate_Weight) / 100));

        resurrectCount += PassiveManager.Instance._MonsterTypeResurrect_Weight[(int)monsterType];
    }

    public override void Init()
    {
        base.Init();
        if(GameManager.Instance.IsInit)
            AudioManager.Instance.Play3DSound(summonSound, transform.position, SettingManager.Instance._FxVolume);
        ResetNode();

        monsterIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.battler_Table, "id");
        if(monsterIndex != -1)
        {
            InitStats(monsterIndex);

            monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), DataManager.Instance.battler_Table[monsterIndex]["type"].ToString());
            holdBackCount = Convert.ToInt32(DataManager.Instance.battler_Table[monsterIndex]["holdbackCount"]);
            requiredMana = Convert.ToInt32(DataManager.Instance.battler_Table[monsterIndex]["requiredMagicpower"]);

            ModifyPassive();
            curHp = maxHp;
        }

        GameManager.Instance.SetMonseter(this, true);

        if(isHide)
            InitState(this, FSMHide.Instance);
        else
            InitState(this, FSMPatrol.Instance);
    }


    public override void Update()
    {
        base.Update();
    }
}
