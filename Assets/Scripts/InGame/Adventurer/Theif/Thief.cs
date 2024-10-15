using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Adventurer, IHide
{
    public virtual bool canAttackbyTrap { get => true; }

    public override void GetDamage(int damage, Battler attacker)
    {
        int curHp = this.curHp;
        base.GetDamage(damage, attacker);

        if (this.curHp < curHp && (object)CurState == FSMHide.Instance)
        {
            ChangeState(FSMPatrol.Instance);
            GetCC(attacker, 0.5f * GameManager.Instance.DefaultSpeed);
        }
    }

    public void HideAction()
    {
        if(CurTile == NodeManager.Instance.endPoint)
            ChangeState(FSMPatrol.Instance);
        else
            Patrol();
    }

    public override void Init()
    {
        base.Init();
        ChangeState(FSMHide.Instance);
        AddStatusEffect<Stealth>(new Stealth(this, 0));
    }

    public override BattlerData GetData()
    {
        BattlerData data = base.GetData();
        data.additionalData = new Dictionary<string, object>();
        data.additionalData.Add("hideState", (object)CurState == FSMHide.Instance);
        return base.GetData();
    }

    public override void LoadData(BattlerData data)
    {
        base.LoadData(data);
        if(data.additionalData != null && data.additionalData.Count > 0)
        {
            bool hideState = System.Convert.ToBoolean(data.additionalData["hideState"]);
            if (!hideState)
                ChangeState(FSMPatrol.Instance);
        }
    }
}
