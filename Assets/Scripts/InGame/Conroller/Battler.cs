using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public interface ISaveLoadBattler
{
    BattlerData GetData();
    void LoadData(BattlerData data);
}

public interface IHide
{
    public bool canAttackbyTrap { get; }
    public void HideAction();
}


public enum AttackType
{
    Melee,
    Ranged,
}

public enum UnitType
{
    Enemy,
    Player,
}

public enum CCType
{
    KnockBack,
}

public class Battler : FSM<Battler>, ISaveLoadBattler
{
    private bool isAnimUniRxSubscribed = false;

    protected CancellationTokenSource unitaskCancelTokenSource = new CancellationTokenSource();

    protected string _name;

    protected int minDamage;
    protected int maxDamage;
    public int Damage { get { return UnityEngine.Random.Range(minDamage, maxDamage + 1); } }
    public int TempDamage(int baseDamage)
    {
        int resultDamage = baseDamage;

        float damageRate = 0;
        foreach (var item in _effects)
        {
            if (item is IAttackPowerRateEffect rateEffect)
                damageRate += rateEffect.attackRate;
        }

        int bonusDamage = 0;
        foreach (var item in _effects)
        {
            if (item is IAttackPowerEffect effect)
                bonusDamage += effect.attackDamage;
        }

        resultDamage *= Mathf.FloorToInt(1 + (damageRate / 100));
        resultDamage += bonusDamage;

        return resultDamage;
    }

    public int curHp;
    public int maxHp;
    public int armor;
    public int shield;
    public float attackSpeed { get; protected set; } = 1;
    public float TempAttackSpeed(float baseAttackSpeed)
    {
        float attackSpeedRate = 0;
        foreach (var item in _effects)
        {
            if (item is IAttackSpeedEffect rateEffect)
                attackSpeedRate += rateEffect.attackSpeedRate;
        }

        return baseAttackSpeed * (1 + (attackSpeedRate / 100));
    }


    public float attackRange;
    public float attackCoolTime = 0;
    public float curAttackCoolTime = 0;

    protected float moveSpeed;

    public bool splashAttack = false;
    public int splashDamage;
    public float splashRange;

    public UnitType unitType = UnitType.Enemy;

    [SerializeField]
    private AttackType attackType = AttackType.Melee;

    private HpBar hpBar;

    protected List<CCType> cc_Emmunes = new List<CCType>();

    public ReactiveCollection<StatusEffect> _effects { get; private set; } = new ReactiveCollection<StatusEffect>();

    [SerializeField]
    private Transform rotatonAxis;

    public Transform RotationAxis { get => rotatonAxis; }

    [SerializeField]
    private Transform hpPivot;

    public Battler chaseTarget;
    public Battler curTarget;
    public Battler prevTarget;

    public Vector3 lastPoint;

    public bool isDead = false;

    protected HashSet<TileNode> crossedNodes = new HashSet<TileNode>();
    protected TileNode _prevNode;
    protected TileNode _curNode;
    protected TileNode _nextNode;
    public TileNode curNode { get => _curNode; }
    public TileNode NextTile { get => _nextNode; set => _nextNode = value; }
    protected TileNode lastCrossRoad;
    protected bool directPass = false;
    public TileNode directPassNode { get; protected set; } = null;

    protected Animator animator;
    public Animator _Animator { get => animator; }

    [SerializeField]
    private Transform attackZone;

    protected HashSet<Battler> rangedTargets = new HashSet<Battler>();

    private int prevRotLevel = -1;
    private Coroutine rotLevelCoroutine = null;

    [SerializeField]
    protected string battlerID;

    [SerializeField]
    protected GameObject attackEffect;

    public string BattlerID { get => battlerID; }

    private float ccTime = 0;

    public virtual float MoveSpeed
    {
        get
        {
            //float slowRate = Mathf.Clamp(1 - PassiveManager.Instance.GetSlowRate(curTile), 0, Mathf.Infinity);
            float tileSpeedMult = unitType == UnitType.Player ? _curNode.curTile.tileAllySpeedMult : _curNode.curTile.tileEnemySpeedMult;
            return moveSpeed * tileSpeedMult;
        }
    }

    public bool HaveEffect<T>() where T : StatusEffect
    {
        foreach (var item in _effects)
        {
            if(item is T)
                return true;
        }
        return false;
    }

    public bool HaveEffect<T>(out T statusEffect) where T : StatusEffect
    {
        statusEffect = null;
        foreach (var item in _effects)
        {
            if (item is T targetEffect)
            {
                statusEffect = targetEffect;
                return true;
            }
        }
        return false;
    }

    public void RemoveStatusEffect<T>() where T : StatusEffect
    {
        if(HaveEffect<T>(out T effect))
            RemoveStatusEffect(effect);
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        if(_effects.Contains(effect))
        {
            effect.DeActiveEffect();
            _effects.Remove(effect);
        }
    }

    public void AddStatusEffect<T>(StatusEffect effect) where T : StatusEffect
    {
        T targetEffect;
        if (HaveEffect<T>(out targetEffect))
            targetEffect.UpdateEffect(effect._originDuration);
        else
            _effects.Add(effect);
    }

    public bool CCEscape()
    {
        ccTime -= GameManager.Instance.InGameDeltaTime;
        if (ccTime <= 0)
            return true;

        return false;
    }

    public void GetCC(Battler attacker, float time)
    {
        this.ccTime = time;
        this.ChangeState(FSMCC.Instance);
        _Animator.SetBool("Move", false);
    }


    public void KnockBack(Battler attacker, TileNode targetTile, Vector3 destPos, float lerpTime)
    {
        _curNode = targetTile;
        GetCC(attacker, lerpTime + 30);
        ExcuteMoveToPos(destPos, lerpTime).Forget();
    }

    public void ResetNode()
    {
        crossedNodes.Clear();
        _prevNode = null;
        //curTile = null;
        _nextNode = null;
        lastCrossRoad = null;
        directPass = false;
    }

    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    protected virtual void RemoveBody()
    {
        gameObject.transform.position = Vector3.up * 1000f;
        _effects.Clear();
        animator?.Rebind();
        Invoke("SetActiveFalse", 0.1f);
    }

    public virtual void Dead()
    {
        hpBar?.UpdateHp();
        isDead = true;

        foreach (var item in _effects)
            item.DeActiveEffect();
        _effects.Clear();

        unitaskCancelTokenSource.Cancel();
        unitaskCancelTokenSource.Dispose();
        unitaskCancelTokenSource = new CancellationTokenSource();
        StopAllCoroutines();
        Invoke("RemoveBody", 2.5f);
        if(GameManager.Instance.holdBackedABattlers.Contains(this))
            GameManager.Instance.holdBackedABattlers.Remove(this);
    }

    private void UpdateChaseTarget(Battler attacker)
    {
        if (attacker == null)
            return;

        //if (attacker.attackType == AttackType.Melee)
        //    return;

        if(chaseTarget == null)
            chaseTarget = attacker;
        else
        {
            float originDist = PathFinder.GetBattlerDistance(this, chaseTarget);
            float updateDist = PathFinder.GetBattlerDistance(this, attacker);
            if (updateDist < originDist)
                chaseTarget = attacker;
        }
    }

    protected void PlayDamageText(int damage, UnitType unitType, bool isCritical)
    {
        Color color = Color.white;
        float fontSize = 27f;
        if(unitType == UnitType.Enemy)
        {
            if (isCritical)
                color = new Color(0.3882353f, 0, 0, 1);
            else
                color = Color.red;
        }
        else if(unitType == UnitType.Player)
        {
            if (isCritical)
                color = new Color(1, 0.9607843f, 0, 1);
            else
                color = Color.white;
        }

        if (isCritical)
            fontSize = 34f;

        DamageTextPooling.Instance.TextEffect(transform.position, damage, fontSize, color, isCritical);
    }

    public virtual void GetHeal(int heal, Battler healer)
    {
        if (isDead)
            return;
        if (healer != null && healer.isDead)
            return;

        curHp = Mathf.Min(curHp + heal, maxHp);
        const float fontSize = 27f;
        DamageTextPooling.Instance.TextEffect(transform.position, heal, fontSize, Color.green, false, true);
    }

    public virtual void GetDamage(int damage, Battler attacker)
    {
        if (isDead)
            return;
        if (attacker != null && attacker.isDead)
            return;

        bool isCritical = false;

        int finalDamage = damage - armor;
        if (finalDamage <= 0)
            finalDamage = 1;

        PlayDamageText(finalDamage, unitType, isCritical);

        if(shield >= finalDamage)
            shield -= finalDamage;
        else
        {
            curHp -= Mathf.Abs(shield - finalDamage);
            shield = 0;
            if (curHp <= 0)
                Dead();
        }

        if ((object)CurState == FSMPatrol.Instance)
        {
            UpdateChaseTarget(attacker);
            ChangeState(FSMChase.Instance);
        }
    }

    public void RotateCharacter(Vector3 targetPos)
    {
        if (isDead)
            return;

        Vector3 targetDirection = targetPos - transform.position;
        if (attackZone != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            attackZone.rotation = targetRotation;
        }

        if (rotatonAxis != null)
        {
            float rotationX = UtilHelper.NormalizeAngle(rotatonAxis.rotation.eulerAngles.x);

            if (targetDirection.x < 0f)
            {
                if (rotationX > 0)
                    rotationX *= -1f;
                rotatonAxis.rotation = Quaternion.Euler(rotationX, 180, rotatonAxis.rotation.eulerAngles.z);
            }
            else
            {
                if (rotationX < 0)
                    rotationX *= -1f;
                rotatonAxis.rotation = Quaternion.Euler(rotationX, 0, rotatonAxis.rotation.eulerAngles.z);
            }
        }
    }

    private void TileMoveCheck(TileNode nextNode, float distance)
    {
        if (_curNode == nextNode)
            return;

        if(distance < Vector3.Distance(_curNode.transform.position, transform.position))
        {
            _prevNode = _curNode;
            _curNode = nextNode;
        }
    }

    protected virtual TileNode FindNextNode(TileNode curNode)
    {
        //startNode에서 roomDirection이나 pathDirection이 있는 방향의 이웃노드를 받아옴
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(_prevNode);
        nextNodes.Remove(NodeManager.Instance.startPoint);
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

    protected virtual void NodeAction(TileNode nextNode)
    {
        this._nextNode = null;
        crossedNodes.Add(_curNode);
        transform.position = _curNode.transform.position;

        // 마왕성 이동 중 유효타일이 발생시 directPass = false;
        if (directPass && FindNextNode(_curNode) != null)
            directPass = false;
    }

    protected async UniTaskVoid ExcuteMoveToPos(Vector3 destPos, float lerpTime)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        while(elapsedTime < lerpTime)
        {
            elapsedTime += GameManager.Instance.InGameDeltaTime;
            Vector3 nextPos = Vector3.Lerp(startPos, destPos, elapsedTime / lerpTime);
            nextPos.y += (Mathf.Sin(elapsedTime / lerpTime * Mathf.PI) * 0.2f);
            transform.position = nextPos;

            await UniTask.Yield(unitaskCancelTokenSource.Token);
        }

        transform.position = destPos;
    }

    protected void ExcuteMove(TileNode nextNode)
    {
        Vector3 nextPos = nextNode.transform.position;
        RotateCharacter(nextPos);
        transform.position = Vector3.MoveTowards(transform.position, nextPos, MoveSpeed * Time.deltaTime * GameManager.Instance.timeScale);
        float distance = Vector3.Distance(transform.position, nextPos);
        TileMoveCheck(nextNode, distance);
    }

    protected void ResetPaths()
    {
        crossedNodes.Clear();
        lastCrossRoad = null;
        _prevNode = null;
        directPass = false;
    }

    public void DirectPass()
    {
        DirectPass(directPassNode);
    }

    protected virtual void DirectPass(TileNode targetTile)
    {
        //마왕타일로 이동수행
        if (_nextNode == null || _nextNode.curTile == null)
        {
            List<TileNode> path = PathFinder.FindPath(_curNode, targetTile);

            //이미 다음노드로 이동중이었다면 현재노드는 제외
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

    protected void NormalPatrol()
    {
        //다음 노드가 없으면 다음노드 탐색
        if (_nextNode == null || _nextNode.curTile == null)
            _nextNode = FindNextNode(_curNode);

        //탐색후에도 유효한 노드가 없을경우
        if (_nextNode == null)
        {
            directPass = true;
            return;
        }

        // nextNode로 이동수행
        ExcuteMove(_nextNode);

        // nextNode까지 이동완료
        if (Vector3.Distance(transform.position, _nextNode.transform.position) < 0.001f)
            NodeAction(_nextNode);
    }

    protected float collapseCool = 0;

    public virtual void UseSkill() { }

    private void ReturnToBase()
    {
        StopAllCoroutines();
        hpBar.HPBarEnd();
        ChangeState(FSMReturn.Instance);
        isDead = true;
        Invoke("RemoveBody", 2.5f);
        //if (deadSound != null)
        //    AudioManager.Instance.Play2DSound(deadSound, SettingManager.Instance._FxVolume);
        GameManager.Instance.waveController.AddDelayedTarget(_name);
    }

    protected bool isCollapsed = false;

    public void CheckTargetCollapsed()
    {
        isCollapsed = PathFinder.FindPath(_curNode, NodeManager.Instance.endPoint) == null;
    }

    protected virtual void CollapseLogic()
    {
        if (isCollapsed)
        {
            collapseCool += Time.deltaTime * GameManager.Instance.timeScale;
            if (collapseCool >= 5f)
            {
                collapseCool = 0f;
                ReturnToBase();
            }
        }
    }

    public virtual void Patrol()
    {
        CollapseLogic();

        if (directPass)
            DirectPass(NodeManager.Instance.endPoint);
        else
            NormalPatrol();
    }

    public void Chase()
    {
        DirectPass(chaseTarget._curNode);
    }

    protected Battler FindNextTarget(Battler prevTarget = null)
    {
        rangedTargets.Remove(prevTarget);
        if (rangedTargets.Count == 0)
            return null;
        else
        {
            Battler closestTarget = null;
            float dist = Mathf.Infinity;
            foreach(Battler target in rangedTargets)
            {
                float targetDist = (transform.position - target.transform.position).magnitude;
                if(targetDist < dist)
                {
                    dist = targetDist;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }
    }

    private bool IsTargetEscaped()
    {
        Collider collider = curTarget.GetComponent<Collider>();
        float dist = Vector3.Distance(collider.ClosestPoint(curTarget.transform.position), transform.position);
        if (dist > attackRange + 0.1f)
            return true;

        return false;
    }

    protected void AttackEndCheck()
    {
        if (curTarget == null || curTarget.isDead)
        {
            if (animator != null)
            {
                animator.SetBool("Attack", false);
                UpdateAttackSpeed();
            }
            return;
        }
    }

    public virtual void Play_AttackAnimation()
    {
        if (animator != null)
        {
            UpdateAttackSpeed();
            animator.SetTrigger("Attack");
        }
    }

    private void SplashAttack(Battler mainTarget, int baseDamage)
    {
        List<Battler> splashTargets = GetRangedTargets(mainTarget.transform.position, splashRange, false);
        splashTargets.Remove(mainTarget);
        int damage = Mathf.CeilToInt((float)baseDamage * splashDamage / 100f);
        foreach (Battler target in splashTargets)
            target.GetDamage(damage, this);
    }

    //애니메이션 이벤트에서 작동
    public virtual void Attack()
    {
        if (attackEffect != null)
            EffectPooling.Instance.PlayEffect(attackEffect, curTarget.transform, Vector3.zero, 0.9f);

        if (curTarget != null && !curTarget.isDead)
        {
            int baseDamage = Damage;
            int tempDamage = TempDamage(baseDamage);
            curTarget.GetDamage(tempDamage, this);
            if (splashAttack)
                SplashAttack(curTarget, tempDamage);
        }
    }

    public void SetRotation()
    {
        if (rotatonAxis != null)
        {
            rotatonAxis.rotation = TargetRoation(1);
            //prevRotLevel = GameManager.Instance.cameraController.Camera_Level;
        }
    }

    protected virtual void InitStats(int index)
    {
        _name = DataManager.Instance.battler_Table[index]["name"].ToString();
        maxHp = Convert.ToInt32(DataManager.Instance.battler_Table[index]["hp"]);
        curHp = maxHp;
        shield = 0;
        minDamage = Convert.ToInt32(DataManager.Instance.battler_Table[index]["attackPowerMin"]);
        maxDamage = Convert.ToInt32(DataManager.Instance.battler_Table[index]["attackPowerMax"]);
        float tempAttackSpeed = 1f;
        float.TryParse(DataManager.Instance.battler_Table[index]["attackSpeed"].ToString(), out tempAttackSpeed);
        attackSpeed = tempAttackSpeed;
        armor = Convert.ToInt32(DataManager.Instance.battler_Table[index]["armor"]);
        float.TryParse(DataManager.Instance.battler_Table[index]["moveSpeed"].ToString(), out moveSpeed);

        if(float.TryParse(DataManager.Instance.battler_Table[index]["attackRange"].ToString(), out attackRange))
            attackRange += UnityEngine.Random.Range(-0.01f, 0.01f);

        float.TryParse(DataManager.Instance.battler_Table[index]["splashRange"].ToString(), out splashRange);
        if (splashRange > 0)
        {
            splashDamage = Convert.ToInt32(DataManager.Instance.battler_Table[index]["splashPower"]);
            splashAttack = true;
        }
    }

    public virtual void Init()
    {
        isDead = false;
        chaseTarget = null;
        curTarget = null;
        isCollapsed = false;
        hpBar = HPBarPooling.Instance.GetHpBar(unitType, this);
        _effects.Clear();
        SetRotation();
        moveSpeed = 1;
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        animator.SetBool("Move", GameManager.Instance.timeScale != 0);
        if (animator != null && !isAnimUniRxSubscribed)
        {
            isAnimUniRxSubscribed = true;
            SubscribeAnimation();
        }

        if (_curNode == null)
        {
            _curNode = NodeManager.Instance.startPoint;
            transform.position = _curNode.transform.position;
        }

        RandomizePosition();
    }

    private void RandomizePosition()
    {
        float targetZ = UnityEngine.Random.Range(-0.001f, 0.001f);
        animator.transform.localPosition = new Vector3(0, 0, targetZ);
    }

    private Quaternion TargetRoation(int camera_Level)
    {
        Vector3 characterRotation = UtilHelper.NormalizeEulerAngles(rotatonAxis.eulerAngles);

        switch (camera_Level)
        {
            case 0:
                characterRotation.x = 70f;
                break;
            case 1:
                characterRotation.x = 70f;
                break;
            case 2:
                characterRotation.x = 50f;
                break;
            case 3:
                characterRotation.x = 20f;
                break;
        }

        if (characterRotation.y > 90f)
        {
            characterRotation.x *= -1f;
            characterRotation.y = 180f;
        }

        return Quaternion.Euler(characterRotation);
    }

    private Vector3 UpdateStartRotation(Vector3 originRotation)
    {
        Vector3 characterRotation = UtilHelper.NormalizeEulerAngles(rotatonAxis.eulerAngles);

        if (Mathf.Abs(originRotation.y - characterRotation.y) < 1f)
            return originRotation;
        else
            return new Vector3(-(originRotation.x), characterRotation.y, originRotation.z);

    }

    private IEnumerator IUpdateRotation()
    {
        if (rotatonAxis != null)
        {
            int camera_Level = GameManager.Instance.cameraController.Camera_Level;
            Vector3 startRotation = UtilHelper.NormalizeEulerAngles(rotatonAxis.rotation.eulerAngles);
            Vector3 targetRotation = UtilHelper.NormalizeEulerAngles(TargetRoation(camera_Level).eulerAngles);

            float elapsedTime = 0f;
            float lerpTime = 1f;

            while (elapsedTime < lerpTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / lerpTime);

                startRotation = UpdateStartRotation(startRotation);
                targetRotation = UtilHelper.NormalizeEulerAngles(TargetRoation(camera_Level).eulerAngles);

                Vector3 nextRotation = Vector3.Lerp(startRotation, targetRotation, t);
                rotatonAxis.rotation = Quaternion.Euler(nextRotation);
                yield return null;
            }

            //rotatonAxis.rotation = Quaternion.Euler(targetRotation);
        }
    }

    private void UpdateRotation()
    {
        if (rotLevelCoroutine != null)
            StopCoroutine(rotLevelCoroutine);

        rotLevelCoroutine = StartCoroutine(IUpdateRotation());
    }

    private bool IsTargetValid(Battler target)
    {
        float attackRange = this.attackRange;
        AttackType attackType = this.attackType;
        
        if (target.tag == "King")
        {
            //타겟이 마왕일경우 : 사거리가 0.5f인 근거리 유닛으로 처리
            attackRange = 0.5f;
            attackType = AttackType.Melee;
        }

        if (target.curNode == null)
            return false;

        if (PathFinder.GetBattlerDistance(this, target) > attackRange)
            return false;

        return true;
    }

    private void UpdateAttackSpeed()
    {
        if (animator != null)
            animator.SetFloat("AttackSpeed", TempAttackSpeed(attackSpeed) * GameManager.Instance.timeScale);
    }

    public List<Battler> GetRangedTargets(Vector3 position, float attackRange, bool attackRangeCheck = true)
    {
        List<Battler> validTargets = new List<Battler>();
        Collider[] colliders = new Collider[10];
        int colliderCount = Physics.OverlapSphereNonAlloc(position, attackRange, colliders, LayerMask.GetMask("Character"));
        for (int i = 0; i < colliderCount; i++)
        {
            Battler battle = colliders[i].GetComponent<Battler>();
            if (battle == null || battle == this)
                continue;
            if (battle.unitType == unitType || battle.isDead)
                continue;
            if(attackRangeCheck && !IsTargetValid(battle))
                continue;
            validTargets.Add(battle);
        }
        return validTargets;
    }

    protected virtual void RemoveOutCaseTargets(List<Battler> targets)
    {
        List<Battler> removeList = new List<Battler>();
        foreach (Battler battler in rangedTargets)
        {
            if (battler.HaveEffect<Stealth>() && !this.HaveEffect<Detect>())
                removeList.Add(battler);
            else if (battler.isDead || !targets.Contains(battler))
                removeList.Add(battler);
        }

        foreach (Battler battler in removeList)
            rangedTargets.Remove(battler);
    }

    private void ModifyBattlerList(List<Battler> targets, bool holdBackCheck)
    {
        foreach (Battler battler in targets)
        {
            rangedTargets.Add(battler);
            
            if(holdBackCheck && battler is IHoldbacker holdbacker)
            {
                //해당 개체가 저지가능상태가 아니고, this를 저지하고 있는 중이 아니라면 대상에서 제외시킨다.
                if (!holdbacker.CanHoldBack && !holdbacker.holdBackTargets.Contains(this))
                    rangedTargets.Remove(battler);
            }
        }

        RemoveOutCaseTargets(targets);
    }

    public Battler BattleCheck(bool holdBackCheck = true)
    {
        //Battler curTarget = null;
        List<Battler> curTargets = GetRangedTargets(transform.position, attackRange);
        ModifyBattlerList(curTargets, holdBackCheck);
        //foreach(Battler battle in rangedTargets)
        //{
        //    if (curTarget == null) //사거리 내에 들어온 유일한 타겟일경우에만 지정가능하도록한다.
        //        curTarget = battle;
        //    else if (Vector3.Distance(transform.position, battle.transform.position) <
        //        Vector3.Distance(transform.position, curTarget.transform.position) && battle.tag != "King")
        //        curTarget = battle;
        //}

        Battler target = GetPriorityTarget();
        if(target is IHoldbacker holdbackable && !GameManager.Instance.holdBackedABattlers.Contains(this))
            holdbackable.AddHoldBackTarget(this);
        return target;
    }

    protected virtual Battler GetPriorityTarget()
    {
        //전에 때렸던 타겟이 사거리 내에 있다면 최우선타겟으로 삼는다.
        if(prevTarget != null && rangedTargets.Contains(prevTarget))
            return prevTarget;

        Battler curTarget = null;
        foreach (Battler battle in rangedTargets)
        {
            if (curTarget == null)
                curTarget = battle;
            else if (Vector3.Distance(transform.position, battle.transform.position) <
                Vector3.Distance(transform.position, curTarget.transform.position) && battle.tag != "King")
                curTarget = battle;
        }
        return curTarget;
    }

    public virtual void Update()
    {
        if (hpBar != null)
            hpBar.UpdateHpBar(hpPivot.position);

        if(prevRotLevel != GameManager.Instance.cameraController.Camera_Level)
        {
            prevRotLevel = GameManager.Instance.cameraController.Camera_Level;
            //UpdateRotation();
        }

        FSMUpdate();
    }

    public virtual void LoadData(BattlerData data)
    {
        _curNode = NodeManager.Instance.FindNode(data.row, data.col);
        if(data.nextRow != -1)
            _nextNode = NodeManager.Instance.FindNode(data.nextRow, data.nextCol);

        if (data.lastCrossedRow != -1)
            lastCrossRoad = NodeManager.Instance.FindNode(data.lastCrossedRow, data.lastCrossedCol);

        for (int i = 0; i < data.crossedRow.Count; i++)
            crossedNodes.Add(NodeManager.Instance.FindNode(data.crossedRow[i], data.crossedCol[i]));

        transform.position = new Vector3(data.pos_x, transform.position.y, data.pos_z);
        curHp = data.curHp;
        maxHp = data.maxHp;
        hpBar.UpdateHp();
    }

    public virtual BattlerData GetData()
    {
        BattlerData target = new BattlerData();
        target.id = battlerID;
        target.curHp = curHp;
        target.maxHp = maxHp;
        target.pos_x = transform.position.x;
        target.pos_z = transform.position.z;

        target.row = _curNode.row;
        target.col = _curNode.col;

        target.crossedRow = new List<int>();
        target.crossedCol = new List<int>();
        foreach (TileNode node in crossedNodes)
        {
            target.crossedRow.Add(node.row);
            target.crossedCol.Add(node.col);
        }
        
        target.nextRow = _nextNode != null ? _nextNode.row : -1;
        target.nextCol = _nextNode != null ? _nextNode.col : -1;

        target.lastCrossedRow = lastCrossRoad != null ? lastCrossRoad.row : -1;
        target.lastCrossedCol = lastCrossRoad != null ? lastCrossRoad.col : -1;

        return target;
    }

    private void SubscribeAnimation()
    {
        GameManager.Instance._timeScale.Where(_ => animator.ContainsParam("Move"))
            .Subscribe(_ =>
            {
                animator.SetBool("Move", _ != 0);
            }).AddTo(gameObject);

        GameManager.Instance._timeScale.Subscribe(_ =>
        {
            UpdateAttackSpeed();
        }).AddTo(gameObject);

        _effects.ObserveCountChanged().Subscribe(_ =>
        {
            UpdateAttackSpeed();
        }).AddTo(gameObject);
    }

    protected void SubscribeStateAnimation()
    {
        _CurState.Where(_ => animator.ContainsParam("Move")).
            Subscribe(_ => animator.SetBool("Move", (object)_ == FSMPatrol.Instance || FSMChase.Instance || FSMDirectMove.Instance));
    }
}
