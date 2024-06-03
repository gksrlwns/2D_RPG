using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    public bool isAttacking;
    public float attackSpeed;
    Dictionary<SecondsType, WaitForSeconds> waitForSecondsDict;
    Animator animator;
    enum SecondsType { AttackSpeed, Attack_Anim};
    private void Awake()
    {
        animator = GetComponent<Animator>();
        waitForSecondsDict = new Dictionary<SecondsType, WaitForSeconds>();
    }
    private void Start()
    {
        waitForSecondsDict.Add(SecondsType.AttackSpeed, new WaitForSeconds(attackSpeed));
        waitForSecondsDict.Add(SecondsType.Attack_Anim, new WaitForSeconds(GetAnimationLength("attack")));
    }

    public void OnMove(Vector3 moveVec)
    {
        animator.SetFloat("Speed", moveVec.magnitude);
    }
    public void OnAttack()
    {
        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        //Debug.Log("몬스터 기본 공격 사용");
        yield return waitForSecondsDict[SecondsType.Attack_Anim];

        yield return waitForSecondsDict[SecondsType.AttackSpeed];
        isAttacking = false;
    }

    public void OnDead()
    {
        //animator.SetTrigger("Die");
        animator.SetBool("isDead", true);
    }
    public void OnRevive()
    {
        animator.SetBool("isDead", false);
    }

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
