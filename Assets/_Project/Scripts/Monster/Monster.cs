using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum MonsterType { Basic, Boss }
    [Header("몬스터  Status")]
    public float maxHp = 100;
    public float damage = 10;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public float exp = 10f;
    public MonsterType monsterType = MonsterType.Basic;
    [SerializeField] float speed = 3f;
    [SerializeField] Vector3 localScale = new Vector3(3f, 3f, 3f);
    [Header("몬스터 현재 Status")]
    public float curHp;
    [Header("Test")]
    [SerializeField] Vector2 moveVec;
    [SerializeField] TargetSearch targetSearch;
    [SerializeField] Vector2 patrolDestination;
    Rigidbody2D rigidbody;
    MonsterAnimator monsterAnimator;
    MonsterWeapon monsterWeapon;
    MonsterEffect monsterEffect;

    [SerializeField] bool isStun = false;
    [SerializeField] bool isLive = true;
    bool isAttackRange;
    bool isPatrol;
    bool isRest;
    [SerializeField] float patrolRestTime = 2f;
    [SerializeField] float knightStunTime = 1f;

    
    float targetDistance;
    WaitForSeconds stunWaitForSeconds;
    WaitForSeconds patrolWaitForSeconds;
    
    int deadLayerNum;
    int liveLayerNum;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        targetSearch = GetComponent<TargetSearch>();
        monsterWeapon = GetComponentInChildren<MonsterWeapon>();
        monsterAnimator = GetComponentInChildren<MonsterAnimator>();
        monsterEffect = GetComponentInChildren<MonsterEffect>();
        stunWaitForSeconds = new WaitForSeconds(knightStunTime);
        patrolWaitForSeconds = new WaitForSeconds(patrolRestTime);
        deadLayerNum = LayerMask.NameToLayer("Dead");
        liveLayerNum = gameObject.layer;
        
        monsterAnimator.attackSpeed = attackSpeed;
    }

    private void Update()
    {
        if (!isLive) return;
        if(curHp <= 0f)
        {
            Die();
            return;
        }
        if(isStun) return;
        if (targetSearch.target) targetDistance = Vector2.Distance(targetSearch.target.transform.position, transform.position);
        
        Move();
        Rotate();
        Attack();

    }
    private void FixedUpdate()
    {
        rigidbody.velocity = Vector2.zero;
    }
    public void Init(MonsterData monsterData)
    {
        maxHp = monsterData.maxHp;
        curHp = maxHp;
        damage = monsterData.damage;
        attackRange = monsterData.attackRange;
        attackSpeed = monsterData.attackSpeed;
        gameObject.layer = liveLayerNum;
        isLive = true;
        curHp = maxHp;
        monsterAnimator.OnRevive();
    }
    public void InitBoss(MonsterData monsterData)
    {
        maxHp = monsterData.maxHp * 3;
        curHp = maxHp;
        exp *= 10;
        damage = monsterData.damage * 3;
        attackRange = monsterData.attackRange * 3;
        attackSpeed = monsterData.attackSpeed;
    }
    void Move()
    {
        monsterAnimator.OnMove(moveVec);
        if (isAttackRange && targetSearch.target)
        {
            moveVec = Vector2.zero;
            return;
        }
        if (targetSearch.target)
        {
            isPatrol = false;
            Vector2 dir = targetSearch.target.transform.position - transform.position;
            moveVec = dir.normalized * speed * Time.deltaTime;
        }
        else if(!isRest)
        {
            if (!isPatrol)
            {
                patrolDestination = PatrolStart();
            }
            else
            {
                Vector2 dir = patrolDestination - (Vector2)transform.position;
                moveVec = dir.normalized * speed * Time.deltaTime;
                if(Vector2.Distance(transform.position,patrolDestination) < 0.1f)
                {
                    isPatrol = false;
                    moveVec = Vector2.zero;
                    isRest = true;
                    StartCoroutine(PatrolReset());
                }
            }
        }

        rigidbody.MovePosition(rigidbody.position + moveVec);
    }
    IEnumerator PatrolReset()
    {
        yield return patrolWaitForSeconds;
        isRest = false;
    }
    Vector2 PatrolStart()
    {
        isPatrol = true;
        float randomX = Random.Range(-2f, 2f);
        Vector2 dir = (Vector2)transform.position + new Vector2(randomX, 0);
        return dir;
    }
    void Rotate()
    {
        if (monsterType.Equals(MonsterType.Boss))
        {
            if (moveVec.x > 0)
            {
                transform.localScale = new Vector3(-3, 3, 3);
            }
            else if (moveVec.x < 0)
            {
                transform.localScale = new Vector3(3, 3, 3);
            }
        }
        else
        {
            if (moveVec.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (moveVec.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void Attack()
    {
        if (!TargetInAttackRange() || !targetSearch.target) return;
        if (!monsterAnimator.isAttacking)
        {
            monsterAnimator.OnAttack();
            monsterWeapon.SeekAndDamage(damage, targetSearch.target);
            //monsterWeapon.MeleeAttack(damage, targetSearch.target);
        }
    }
    bool TargetInAttackRange()
    {
        if (targetDistance <= attackRange) isAttackRange = true;
        else isAttackRange = false;

        return isAttackRange;
    }
    public void Damaged(float _damage, CharacterData characterData)
    {
        curHp -= _damage;
        if(curHp <= 0)
        {
            characterData.CurExp += exp;
        }
    }
    /// <summary>
    /// 탱커의 스킬이 적용되도록 구현했고, 보스몬스터는 Stun에 걸리지 않도록 했다.
    /// </summary>
    public void Stun()
    {
        if (monsterType.Equals(MonsterType.Boss)) return;
        isStun = true;
        monsterEffect.StunEffect();
        StartCoroutine(StunOut());
    }
    IEnumerator StunOut()
    {
        yield return stunWaitForSeconds;
        isStun = false;
    }
    void Die()
    {
        isLive = false;
        monsterAnimator.OnDead();
        gameObject.layer = deadLayerNum;
        GameManager.instance.monsterSpawner.dieCount++;
        GameManager.instance.coin += Random.Range(1, 3);
        if (monsterType.Equals(MonsterType.Boss)) GameManager.instance.ClearGame();
        StartCoroutine(ReturnPool());

    }
    IEnumerator ReturnPool()
    {
        yield return patrolWaitForSeconds;
        yield return stunWaitForSeconds;
        if(monsterType.Equals(MonsterType.Boss)) Destroy(gameObject);
        else PoolManager.instance.ReturnObject(PoolType.Monster, gameObject);
    }
    public void ForceReturnObject()
    {
        PoolManager.instance.ReturnObject(PoolType.Monster, gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
