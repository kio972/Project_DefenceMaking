using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UIElements;

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

public class Battler : FSM<Battler>
{
    protected string _name;

    protected int minDamage;
    protected int maxDamage;
    public int Damage { get { return UnityEngine.Random.Range(minDamage, maxDamage + 1); } }
    public int curHp;
    public int maxHp;
    public int armor;
    public float attackSpeed;
    public float attackRange;
    protected float moveSpeed;

    public bool splashAttack = false;
    public int splashDamage;
    public float splashRange;

    public UnitType unitType = UnitType.Enemy;

    [SerializeField]
    private AttackType attackType = AttackType.Melee;

    private HpBar hpBar;

    [SerializeField]
    private Transform rotatonAxis;

    public Transform RotationAxis { get => rotatonAxis; }

    [SerializeField]
    private Transform hpPivot;

    public Battler chaseTarget;
    public Battler curTarget;

    public Vector3 lastPoint;

    public bool isDead = false;

    protected List<TileNode> crossedNodes = new List<TileNode>();
    protected TileNode prevTile;
    protected TileNode curTile;
    protected TileNode nextTile;
    public TileNode CurTile { get => curTile; }
    public TileNode NextTile { get => nextTile; }
    protected TileNode lastCrossRoad;
    protected bool directPass = false;

    protected Animator animator;
    public Animator _Animator { get => animator; }

    [SerializeField]
    private Transform attackZone;

    protected List<Battler> rangedTargets = new List<Battler>();

    private int prevRotLevel = -1;
    private Coroutine rotLevelCoroutine = null;

    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private AudioClip deadSound;

    [SerializeField]
    protected string battlerID;

    [SerializeField]
    protected GameObject attackEffect;

    public string BattlerID { get => battlerID; }

    private float ccTime = 0;

    public float MoveSpeed
    {
        get
        {
            float slowRate = Mathf.Clamp(1 - PassiveManager.Instance.GetSlowRate(curTile), 0, Mathf.Infinity);
            return moveSpeed * slowRate;
        }
    }

    public bool CCEscape()
    {
        ccTime -= Time.deltaTime * GameManager.Instance.timeScale;
        if (ccTime <= 0)
            return true;

        return false;
    }

    public void GetCC(Battler attacker, float time)
    {
        this.ccTime = time;
        
    }

    public void ResetNode()
    {
        crossedNodes = new List<TileNode>();
        prevTile = null;
        curTile = null;
        nextTile = null;
        lastCrossRoad = null;
        directPass = false;
    }

    protected virtual void RemoveBody()
    {
        gameObject.SetActive(false);
    }

    public virtual void Dead()
    {
        hpBar?.UpdateHp();
        isDead = true;
        StopAllCoroutines();
        Invoke("RemoveBody", 2.5f);
        if(deadSound != null)
            AudioManager.Instance.Play2DSound(deadSound, SettingManager.Instance._FxVolume);
    }

    private void UpdateChaseTarget(Battler attacker)
    {
        if (attacker == null)
            return;

        if (attacker.attackType == AttackType.Melee)
            return;

        if(chaseTarget == null)
            chaseTarget = attacker;
        else
        {
            float originDist = PathFinder.Instance.GetBattlerDistance(this, chaseTarget);
            float updateDist = PathFinder.Instance.GetBattlerDistance(this, attacker);
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

    public virtual void GetDamage(int damage, Battler attacker)
    {
        if (isDead)
            return;
        if (attacker != null && attacker.isDead)
            return;

        UpdateChaseTarget(attacker);

        bool isCritical = false;

        int finalDamage = damage - armor;
        if (finalDamage <= 0)
            finalDamage = 1;

        PlayDamageText(finalDamage, unitType, isCritical);

        curHp -= finalDamage;
        if (curHp <= 0)
            Dead();
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
        if (curTile == nextNode)
            return;

        if(distance < Vector3.Distance(curTile.transform.position, transform.position))
        {
            prevTile = curTile;
            curTile = nextNode;
        }
    }

    protected virtual TileNode FindNextNode(TileNode curNode)
    {
        //startNode에서 roomDirection이나 pathDirection이 있는 방향의 이웃노드를 받아옴
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(prevTile);
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
        this.nextTile = null;
        if (!crossedNodes.Contains(curTile))
            crossedNodes.Add(curTile);

        // 마왕성 이동 중 유효타일이 발생시 directPass = false;
        if (directPass && FindNextNode(curTile) != null)
            directPass = false;
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
        crossedNodes = new List<TileNode>();
        lastCrossRoad = null;
        prevTile = null;
        directPass = false;
    }

    protected virtual void DirectPass(TileNode targetTile)
    {
        //마왕타일로 이동수행
        if (nextTile == null || nextTile.curTile == null)
        {
            List<TileNode> path = PathFinder.Instance.FindPath(curTile, targetTile);
            if (path != null && path.Count > 0)
                nextTile = path[0];
            else
            {
                ResetPaths();
                return;
            }
        }

        ExcuteMove(nextTile);

        // nextNode까지 이동완료
        if (Vector3.Distance(transform.position, nextTile.transform.position) < 0.001f)
            NodeAction(nextTile);
    }

    protected void NormalPatrol()
    {
        //다음 노드가 없으면 다음노드 탐색
        if (nextTile == null || nextTile.curTile == null)
            nextTile = FindNextNode(curTile);

        //탐색후에도 유효한 노드가 없을경우
        if (nextTile == null)
        {
            directPass = true;
            return;
        }

        // nextNode로 이동수행
        ExcuteMove(nextTile);

        // nextNode까지 이동완료
        if (Vector3.Distance(transform.position, nextTile.transform.position) < 0.001f)
            NodeAction(nextTile);
    }

    protected float collapseCool = 0;

    private void ReturnToBase()
    {
        isDead = true;
        StopAllCoroutines();
        hpBar.HPBarEnd();
        ChangeState(FSMDead.Instance);
        Invoke("RemoveBody", 2.5f);
        if (deadSound != null)
            AudioManager.Instance.Play2DSound(deadSound, SettingManager.Instance._FxVolume);
        GameManager.Instance.waveController.AddDelayedTarget(_name);
    }

    public virtual void Patrol()
    {
        if (PathFinder.Instance.FindPath(curTile, NodeManager.Instance.endPoint) == null)
        {
            collapseCool += Time.deltaTime * GameManager.Instance.timeScale;
            if (collapseCool >= 5f)
            {
                collapseCool = 0f;
                ReturnToBase();
            }
        }

        if (directPass)
            DirectPass(NodeManager.Instance.endPoint);
        else
            NormalPatrol();
    }

    public void Chase()
    {
        DirectPass(chaseTarget.curTile);
    }

    protected Battler FindNextTarget(Battler prevTarget = null)
    {
        rangedTargets.Remove(prevTarget);
        if (rangedTargets.Count == 0)
            return null;
        else
        {
            Battler closestTarget = rangedTargets[0];
            float dist = (transform.position - closestTarget.transform.position).magnitude;
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
                animator.SetFloat("AttackSpeed", attackSpeed * GameManager.Instance.timeScale);
                
            }
            return;
        }
    }

    public void Play_AttackAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("AttackSpeed", attackSpeed * GameManager.Instance.timeScale);
            animator.SetTrigger("Attack");
        }
    }

    private void SplashAttack(Battler mainTarget)
    {
        List<Battler> splashTargets = GetRangedTargets(mainTarget.transform.position, splashRange, false);
        splashTargets.Remove(mainTarget);
        foreach (Battler target in splashTargets)
            target.GetDamage(splashDamage, this);
    }

    //애니메이션 이벤트에서 작동
    public void Attack()
    {
        if (attackSound != null)
            AudioManager.Instance.Play2DSound(attackSound, SettingManager.Instance._FxVolume);

        if (attackEffect != null)
            EffectPooling.Instance.PlayEffect(attackEffect, curTarget.transform, Vector3.zero, 0.9f);

        if (curTarget != null && !curTarget.isDead)
        {
            curTarget.GetDamage(Damage, this);
            if (splashAttack)
                SplashAttack(curTarget);
        }
    }

    public void SetRotation()
    {
        if (rotatonAxis != null)
        {
            rotatonAxis.rotation = TargetRoation(GameManager.Instance.cameraController.Camera_Level);
            prevRotLevel = GameManager.Instance.cameraController.Camera_Level;
        }
    }

    protected virtual void InitStats(int index)
    {
        _name = DataManager.Instance.Battler_Table[index]["name"].ToString();
        maxHp = Convert.ToInt32(DataManager.Instance.Battler_Table[index]["hp"]);
        curHp = maxHp;

        minDamage = Convert.ToInt32(DataManager.Instance.Battler_Table[index]["attackPowerMin"]);
        maxDamage = Convert.ToInt32(DataManager.Instance.Battler_Table[index]["attackPowerMax"]);
        float.TryParse(DataManager.Instance.Battler_Table[index]["attackSpeed"].ToString(), out attackSpeed);
        armor = Convert.ToInt32(DataManager.Instance.Battler_Table[index]["armor"]);
        float.TryParse(DataManager.Instance.Battler_Table[index]["moveSpeed"].ToString(), out moveSpeed);

        float.TryParse(DataManager.Instance.Battler_Table[index]["attackRange"].ToString(), out attackRange);

        float.TryParse(DataManager.Instance.Battler_Table[index]["splashRange"].ToString(), out splashRange);
        if (splashRange > 0)
        {
            splashDamage = Convert.ToInt32(DataManager.Instance.Battler_Table[index]["splashPower"]);
            splashAttack = true;
        }
    }

    public virtual void Init()
    {
        isDead = false;

        hpBar = HPBarPooling.Instance.GetHpBar(unitType, this);
        SetRotation();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (curTile == null)
        {
            curTile = NodeManager.Instance.startPoint;
            transform.position = curTile.transform.position;
        }
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

        if (target.CurTile == null)
            return false;

        if (PathFinder.Instance.GetBattlerDistance(this, target) > attackRange)
            return false;

        //if (attackType == AttackType.Melee)
        //{
        //    //근거리일경우 : 서로의 타일이 길로 이어진 타일인지 확인
        //    if (UtilHelper.CalCulateDistance(transform, target.transform) > attackRange)
        //        return false;

        //}
        //else if(attackType == AttackType.Ranged)
        //{
        //    //원거리일경우 : 서로의 타일 사이에 방의 벽으로 막혀있지 않은지 확인
        //    Vector3 dir = (target.transform.position - transform.position).normalized;
        //    float dist = (target.transform.position - transform.position).magnitude;
        //    Direction direction = UtilHelper.CheckClosestDirection(dir);
        //    if (direction == Direction.None)
        //        return false;
        //    int tileDistance = (int)dist + 1;
        //    TileNode curTileNode = this.curTile;
        //    for(int i = 0; i < tileDistance; i++)
        //    {
        //        //curTile로부터 direction방향의 노드로 이동하며 타겟노드와 일치하는지 확인
        //        //방의 벽으로 막혀있는지 여부 확인 코드 추가 필요
        //        if (curTileNode == target.curTile)
        //            return true;
        //        curTileNode = curTile.neighborNodeDic[direction];
        //    }
        //    return false;
        //}

        return true;
    }

    public void UpdateAttackSpeed()
    {
        if (animator != null)
            animator.SetFloat("AttackSpeed", attackSpeed * GameManager.Instance.timeScale);
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

    private void RemoveOutCaseTargets(List<Battler> targets)
    {
        List<Battler> removeList = new List<Battler>();
        foreach (Battler battler in rangedTargets)
        {
            if (battler.isDead || !targets.Contains(battler))
                removeList.Add(battler);
        }

        foreach (Battler battler in removeList)
            rangedTargets.Remove(battler);
    }

    private void ModifyBattlerList(List<Battler> targets)
    {
        RemoveOutCaseTargets(targets);

        foreach (Battler battler in targets)
        {
            if (rangedTargets.Contains(battler))
                continue;

            if(battler.unitType == UnitType.Player && battler.tag != "King")
            {
                Monster target = battler.GetComponent<Monster>();
                if (!target.CanHoldBack && !target.rangedTargets.Contains(this))
                    continue;
            }

            if(this.unitType == UnitType.Player && this.tag != "King")
            {
                Monster monster = GetComponent<Monster>();
                if(monster.CanHoldBack)
                    rangedTargets.Add(battler);
            }
            else
                rangedTargets.Add(battler);
        }
    }

    public Battler BattleCheck()
    {
        Battler curTarget = null;
        List<Battler> rangedTargets = GetRangedTargets(transform.position, attackRange);
        ModifyBattlerList(rangedTargets);
        foreach(Battler battle in this.rangedTargets)
        {
            if (curTarget == null) //사거리 내에 들어온 유일한 타겟일경우에만 지정가능하도록한다.
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
            UpdateRotation();
        }

        FSMUpdate();
    }
}
