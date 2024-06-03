using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform projectileTr;
    [SerializeField] CharacterData characterData;
    private void Awake()
    {
        characterData = GetComponent<Character>().characterData;
    }
    public void MeleeAttack (float damage, GameObject target)
    {
        if (target == null) return;
        Monster monster = target.GetComponent<Monster>();
        monster.Damaged(damage, characterData);
    }
    public void RangeAttack(CharacterData characterData, GameObject target)
    {
        if (target == null) return;
        Monster monster = target.GetComponent<Monster>();
        Projectile projectile = null;

        switch (characterData.CharacterType)
        {
            case CharacterType.Archer:
                projectile = PoolManager.instance.GetObject(PoolType.Projectile_Arrow).GetComponent<Projectile>();
                break;
            case CharacterType.Priest:
                projectile = PoolManager.instance.GetObject(PoolType.Projectile_EnergyVolt).GetComponent<Projectile>();
                break;
        }
        projectile.transform.position = projectileTr.position;
        projectile.Seek(monster);
        projectile.Damage(characterData.Damage);
        projectile.CharacterData(characterData);
    }
    /// <summary>
    /// 전사,궁수 단일 스킬
    /// </summary>
    /// <param name="characterData"></param>
    /// <param name="target"></param>
    public void UsingSkill(CharacterData characterData, GameObject target)
    {
        if (target == null) return;
        Monster monster = target.GetComponent<Monster>();
        float damage = characterData.Damage;
        if (characterData.CharacterType.Equals(CharacterType.Archer))
        {
            damage *= 2.5f;
            Projectile projectile = PoolManager.instance.GetObject(PoolType.Projectile_ArrowSkill).GetComponent<Projectile>();
            projectile.transform.position = projectileTr.position;
            projectile.Seek(monster);
            projectile.Damage(damage);
            projectile.CharacterData(characterData);
        }
        else
        {
            monster.Stun();
            monster.Damaged(damage, characterData);
        }
        
    }
    /// <summary>
    /// 도적 -> 범위 스킬
    /// </summary>
    /// <param name="characterData"></param>
    /// <param name="targets"></param>
    public void UsingSkill(CharacterData characterData, Monster[] targets)
    {
        if (targets.Length.Equals(0)) return;
        float damage = characterData.Damage;
        
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].Damaged(damage, characterData);
        }
    }
    
    /// <summary>
    /// Priest 스킬 (힐 스킬)
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="characters"></param>
    public void UsingSkill(float damage, Character[] characters)
    {
        Character character = null;
        if (characters.Length.Equals(0)) return;
        float lowestHp = 500f;
        damage *= 2.5f;
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].Equals(null)) continue;
            if(characters[i].curHp <= lowestHp) { character = characters[i]; };
        }
        character.curHp += damage;
        if (character.curHp >= character.maxHp) character.curHp = character.maxHp;
        character.characterEffect.HealEffect();
    }

    




}
