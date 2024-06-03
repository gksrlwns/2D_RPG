using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("캐릭터 Status")]
    public int level;
    public CharacterData characterData;
    [SerializeField] float speed = 3f;
    [SerializeField] SpriteRenderer[] spriteRendererArray;
    [SerializeField] float reviveTime = 5f;
    public bool isLive = true;

    [Header("캐릭터 현재 Status")]
    public float curHp;
    public float maxHp;
    
    public CharacterEffect characterEffect;
    [SerializeField] RectTransform hpBar;
    [SerializeField] Transform body;

    Rigidbody2D rigidbody;
    CharacterAnimator characterAnimator;
    CharacterManager characterManager;
    Weapon weapon;
    TargetSearch targetSearch;
    WaitForSeconds reviveWaitForSeconds;
    WaitForSeconds restWaitForSeconds;

    Vector2 moveVec;

    bool isAttackRange;
    bool isSkillRange;

    float targetDistance;

    int deadLayerNum;
    int liveLayerNum;



    private void Awake()
    {
        rigidbody= GetComponent<Rigidbody2D>();
        characterAnimator = GetComponent<CharacterAnimator>();
        targetSearch = GetComponent<TargetSearch>();
        weapon = GetComponent<Weapon>();
        characterEffect = GetComponent<CharacterEffect>();
        characterManager = GetComponentInParent<CharacterManager>();
        reviveWaitForSeconds = new WaitForSeconds(reviveTime);
        restWaitForSeconds = new WaitForSeconds(1f);
        characterAnimator.characterData = characterData;
        deadLayerNum = LayerMask.NameToLayer("Dead");
        liveLayerNum = gameObject.layer;
        CharacterInfo();
        characterData.Init();
    }

    private void Start()
    {
        Init();
    }
    void CharacterInfo()
    {
        Debug.Log($"직업 : {characterData.CharacterType.ToString()}\n 체력 : {characterData.MaxHp} \n 공격력 : {characterData.Damage}\n 사거리 : {characterData.AttackRange}\n 공격속도 : {characterData.AttackSpeed}" +
            $"\n스킬쿨타임 : {characterData.SkilCoolDownTime}");
    }
    
    private void Update()
    {
        if (!GameManager.instance.isPlay) return;
        if (characterManager.isAllDead)
        {
            AllDead();
            return;
        }
        if (!isLive) return;
        if (GameManager.instance.isClear)
        {
            Victory();
            moveVec = Vector2.zero;
            return;
        }
        
        hpBar.localScale = new Vector3(curHp / maxHp, 1, 1);
        if(characterData.CurExp >= characterData.MaxExp)
        {
            LevelUp();
        }
        if (curHp <= 0f)
        {
            Die();
            return;
        }
        if (targetSearch.target) targetDistance = Vector2.Distance(targetSearch.target.transform.position, transform.position);
        Move();
        Rotate();
        Skill();
        Attack();
    }
    private void FixedUpdate()
    {
        rigidbody.velocity = Vector2.zero;
    }

    /// <summary>
    /// 캐릭터에 대한 Data를 초기화
    /// </summary>
    public void Init()
    {
        characterAnimator.OnInitAnimation();
        curHp = characterData.MaxHp;
        maxHp = characterData.MaxHp;
        characterManager.isAllDead = false;
        isLive = true;
        characterManager.isDeads[(int)characterData.CharacterType] = !isLive;
        gameObject.layer = liveLayerNum;
    }
    /// <summary>
    /// 캐릭터의 움직임
    /// </summary>
    void Move()
    {
        characterAnimator.OnMove(moveVec);
        if ((isAttackRange && isSkillRange) && targetSearch.target)
        {
            moveVec = Vector2.zero;
            return;
        }

        Vector2 dir;

        if (targetSearch.target)
        {
            dir = targetSearch.target.transform.position - transform.position;
        }
        else
        {
            if (!characterManager.characters[0].isLive) dir = Vector2.zero;
            else dir = Vector2.right;
            
            //if (characterData.CharacterType.Equals(CharacterType.Knight))
            //{
            //    dir = Vector2.right;
            //}
            //else
            //{
            //    Transform target = CharacterManager.instance.GetFollowTr((int)characterData.CharacterType - 1);
            //    dir = target.position - transform.position;
            //}
        }
        

        moveVec = dir.normalized * speed * Time.deltaTime;

        rigidbody.MovePosition(rigidbody.position + moveVec);
    }
   /// <summary>
   /// 캐릭터의 회전
   /// </summary>
    void Rotate()
    {
        if (moveVec.x > 0)
        {
            //body.localScale = new Vector3(-1,1,1);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveVec.x < 0)
        {
            //body.localScale = new Vector3(1, 1, 1);
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    /// <summary>
    /// 캐릭터의 공격
    /// </summary>
    void Attack()
    {
        if (!TargetInAttackRange() || !targetSearch.target) return;
        if (characterAnimator.isAttacking) return;
        if (characterAnimator.isUsingSkill || !characterAnimator.isCooldown) return;
        
        characterAnimator.OnAttack();
        
    }
    /// <summary>
    /// 몬스터에게 데미지를 줄 때, 자연스러움을 위해 Animation Event를 사용
    /// </summary>
    public void AnimationAttackEvent()
    {
        switch (characterData.CharacterType)
        {
            case CharacterType.Knight:
            case CharacterType.Thief:
                weapon.MeleeAttack(characterData.Damage, targetSearch.target);
                break;
            case CharacterType.Archer:
            case CharacterType.Priest:
                weapon.RangeAttack(characterData, targetSearch.target);
                break;
        }
    }
    /// <summary>
    /// 캐릭터 스킬
    /// </summary>
    void Skill()
    {
        if (!TargetInSkilRange() || !targetSearch.target) return;
        if (characterAnimator.isUsingSkill || characterAnimator.isCooldown) return;
        characterAnimator.OnUsingSkil();
    }
    /// <summary>
    /// 몬스터에게 데미지를 줄 때, 자연스러움을 위해 Animation Event를 사용
    /// </summary>
    public void AnimationSkillEvent()
    {
        switch (characterData.CharacterType)
        {
            case CharacterType.Knight:
            case CharacterType.Archer:
                weapon.UsingSkill(characterData, targetSearch.target);
                break;
            case CharacterType.Thief:
                var monsters = targetSearch.MonstersInRange(characterData.Damage);
                weapon.UsingSkill(characterData, monsters);
                break;
            case CharacterType.Priest:
                var characters = targetSearch.CharactersInRange(characterData.SkilRange);
                weapon.UsingSkill(characterData.Damage, characters);
                break;
        }
        characterEffect.SkillEffect();
    }
    void Die()
    {
        isLive = false;
        Debug.Log("죽음");
        gameObject.layer = deadLayerNum;
        characterAnimator.OnDead();
        characterManager.isDeads[(int)characterData.CharacterType] = !isLive;
        if (characterManager.CheckAllDead().Equals(true))
        {
            GameManager.instance.FailGame();
        }
        StartCoroutine(Revive());
    }
    IEnumerator Revive()
    {
        yield return reviveWaitForSeconds;
        characterAnimator.OnRevive();
        curHp = maxHp;
        isLive = true;
        characterManager.isDeads[(int)characterData.CharacterType] = !isLive;
        gameObject.layer = liveLayerNum;
        StopAllCoroutines();
    }
    public void Victory()
    {
        characterAnimator.OnVictory();
    }

    public void AllDead()
    {
        StopAllCoroutines();
    }
    
    public void Damaged(float _damage)
    {
        curHp -= _damage;
        curHp = Mathf.Clamp(curHp, 0f, maxHp);
        characterAnimator.OnDamaged();
    }
    /// <summary>
    /// 캐릭터가 몬스터를 처치해 얻은 경험치로 레벨업 진행
    /// 경험치통은 1.5배, 데미지와 체력은 기존에 1.1배
    /// </summary>
    public void LevelUp()
    {
        characterData.CurExp -= characterData.MaxExp;
        characterData.Level++;
        characterData.MaxExp *= 1.5f;
        characterData.Damage *= 1.1f;
        characterData.MaxHp *= 1.1f;
        characterEffect.LevelUpEffect();
    }
    bool TargetInAttackRange()
    {
        if (targetDistance <= characterData.AttackRange) isAttackRange = true;
        else isAttackRange = false;

        return isAttackRange;
    }
    bool TargetInSkilRange()
    {
        if (targetDistance <= characterData.SkilRange) isSkillRange = true;
        else isSkillRange = false;
        return isSkillRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, characterData.SkilRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, characterData.AttackRange);
    }
}
