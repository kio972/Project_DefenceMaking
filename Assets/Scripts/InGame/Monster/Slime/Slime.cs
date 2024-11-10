using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Slime : Monster
{
    private int splitCount = 0;
    private TileNode splitedNode = null;
    private TileNode slime_curNode = null;
    private bool skipedFirst = false;

    private float splitElapsed = 0;

    private CancellationTokenSource source;

    public override void Dead(Battler attacker)
    {
        source.Cancel();
        source.Dispose();
        base.Dead(attacker);
    }

    public void ModifySplitCount(int value)
    {
        splitCount = Mathf.Max(0, splitCount + value);
    }

    public override void Init()
    {
        base.Init();
        splitElapsed = 0;
        splitCount = 0 + PassiveManager.Instance._slimeSplit_Weight.Value;
        RotationAxis.localScale = Vector3.one;
        splitedNode = null;
        slime_curNode = _curNode;
        source = new CancellationTokenSource();
    }

    private bool IsTileChanged()
    {
        if (slime_curNode == _curNode || 0.05f < Vector3.Distance(_curNode.transform.position, transform.position))
            return false;
        slime_curNode = _curNode;
        if(!skipedFirst)
        {
            skipedFirst = true;
            return false;
        }

        return true;
    }

    private List<TileNode> GetNextNodes(TileNode curNode, TileNode prevNode)
    {
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //�ش� ��忡�� ���� ���� ���� ����
        nextNodes.Remove(_prevNode);
        nextNodes.Remove(GameManager.Instance.king.curNode);
        return nextNodes;
    }

    private async UniTaskVoid ExcuteSplit(List<TileNode> nextNodes)
    {
        _Animator.SetTrigger("Split");
        _Animator.SetFloat("AttackSpeed", TempAttackSpeed(attackSpeed) * GameManager.Instance.timeScale);
        splitElapsed = 1.5f;
        while(splitElapsed > 0)
        {
            splitElapsed -= Time.deltaTime * GameManager.Instance.timeScale;
            await UniTask.Yield(source.Token);
        }
        
        splitCount--;
        splitedNode = _curNode;
        bool isOdd = curHp % 2 == 1;
        curHp /= 2;
        maxHp /= 2;
        BattlerData curData = GetData();
        RotationAxis.localScale *= 0.8f;
        TileNode randomNode = nextNodes[Random.Range(0, nextNodes.Count)];
        this._nextNode = randomNode;
        nextNodes.Remove(randomNode);

        Monster monster = BattlerPooling.Instance.SpawnMonster(curData.id, NodeManager.Instance.FindNode(curData.row, curData.col), "id");
        monster.LoadData(curData);
        Slime newSlime = monster.GetComponent<Slime>();
        newSlime.ExcuteCloneMana();
        newSlime.RotationAxis.localScale = RotationAxis.localScale;
        newSlime.splitCount = this.splitCount;
        newSlime.splitedNode = this.splitedNode;
        newSlime._nextNode = nextNodes[Random.Range(0, nextNodes.Count)];

        if (isOdd) { curHp++; maxHp++; }
    }

    private void ExcuteCloneMana()
    {
        GameManager.Instance._CurMana = GameManager.Instance._CurMana - requiredMana;
        requiredMana = 0;
    }

    public override void Patrol()
    {
        if(splitElapsed > 0)
            return;

        base.Patrol();
        if (splitCount <= 0 || curHp < 2)
            return;

        if (!IsTileChanged())
            return;

        List<TileNode> nextNodes = GetNextNodes(slime_curNode, _prevNode);
        if (nextNodes.Count < 2)
            return;

        ExcuteSplit(nextNodes).Forget();
    }

    public override BattlerData GetData()
    {
        BattlerData data = base.GetData();
        data.additionalData = new Dictionary<string, object>();
        data.additionalData.Add("splitCount", splitCount);
        data.additionalData.Add("curScale", RotationAxis.localScale.x);
        return data;
    }

    public override void LoadData(BattlerData data)
    {
        base.LoadData(data);
        splitCount = System.Convert.ToInt32(data.additionalData["splitCount"]);
        float curScale = float.Parse(data.additionalData["curScale"].ToString());
        RotationAxis.localScale = new Vector3(curScale, curScale, curScale);
    }
}
