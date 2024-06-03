using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    
    public CharacterData characterData;
    public bool isAttacking;
    public bool isUsingSkill;
    public bool isCooldown;
    Dictionary<SecondsType, WaitForSeconds> waitForSecondsDict;
    Animator animator;
    enum SecondsType {AttackSpeed, SkillCooldownTime, Attack_Anim, Skill_Anim, Casting_Anim };



    private void Awake()
    {
        animator = GetComponent<Animator>();
        waitForSecondsDict = new Dictionary<SecondsType, WaitForSeconds>();
    }
    private void Start()
    {
        waitForSecondsDict.Add(SecondsType.AttackSpeed, new WaitForSeconds(characterData.AttackSpeed));
        waitForSecondsDict.Add(SecondsType.SkillCooldownTime, new WaitForSeconds(characterData.SkilCoolDownTime));
        waitForSecondsDict.Add(SecondsType.Attack_Anim, new WaitForSeconds(GetAnimationLength("attack")));
        waitForSecondsDict.Add(SecondsType.Casting_Anim, new WaitForSeconds(GetAnimationLength("casting")));
        waitForSecondsDict.Add(SecondsType.Skill_Anim, new WaitForSeconds(GetAnimationLength("Skill")));

    }
    public void OnInitAnimation()
    {
        animator.SetBool("isLive", true);
        animator.SetBool("isVictory", false);
    }
    public void OnMove(Vector3 moveVec)
    {
        animator.SetFloat("Speed", moveVec.magnitude);
    }
    public void OnAttack()
    {
        StartCoroutine(AttackRoutine());
    }
    public void OnUsingSkil()
    {
        StartCoroutine(SkillRoutine());
    }
    public void OnRevive()
    {
        animator.SetBool("isLive", true);
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
       
        //Debug.Log("기본 공격 사용");
        yield return waitForSecondsDict[SecondsType.Attack_Anim];

        yield return waitForSecondsDict[SecondsType.AttackSpeed];
        isAttacking = false;
        //animator.SetBool("isAttacking", isAttacking);
    }
    IEnumerator SkillRoutine()
    {
        isUsingSkill = true;
        animator.SetTrigger("Skill");
        //animator.SetBool("isUsingSkill", true);
        //Debug.Log("스킬 사용");

        // 캐스팅 애니메이션 재생 시간 동안 대기
        yield return waitForSecondsDict[SecondsType.Casting_Anim];

        if(!characterData.CharacterType.Equals(CharacterType.Priest))
        {
            yield return waitForSecondsDict[SecondsType.Skill_Anim];
        }
        isUsingSkill = false;

        isCooldown = true;
        yield return waitForSecondsDict[SecondsType.SkillCooldownTime];
        
        isCooldown = false;
        
    }
    public void OnDamaged()
    {
        if(!isAttacking)
        {
            animator.SetTrigger("Hurt");
        }
    }
    public void OnDead()
    {
        animator.SetBool("isLive", false);
        animator.SetTrigger("Dead");
    }
    public void OnVictory()
    {
        animator.SetBool("isVictory", true);
    }
    /// <summary>
    /// 현재 애니메이터 상태에서 애니메이션 길이를 가져옴
    /// </summary>
    /// <param name="animationName"></param>
    /// <returns></returns>
    float GetAnimationLength(string animationName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

}
