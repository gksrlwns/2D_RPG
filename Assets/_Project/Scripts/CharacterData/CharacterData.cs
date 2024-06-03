using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType { Knight, Thief, Archer, Priest}

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjcet/CharacterData", order = 0 )]
public class CharacterData : ScriptableObject
{
    [Header("초기 데이터")]
    [SerializeField] CharacterType characterType;
    [SerializeField] int initLevel;
    [SerializeField] float initHp;
    [SerializeField] float initDamage;
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] float skilRange;
    [SerializeField] float skilCoolDownTime;
    [SerializeField] float initMaxExp;
    [Header("변동 데이터")]
    [SerializeField] int level;
    [SerializeField] float maxHp;
    [SerializeField] float damage;
    [SerializeField] float curExp;
    [SerializeField] float maxExp;
    public CharacterType CharacterType { get { return characterType; } }
    public int Level { get { return level; } set { level = value; } }
    public float MaxHp { get { return maxHp; } set { maxHp = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float AttackRange {  get { return attackRange; } }
    public float SkilRange { get { return skilRange; } }
    public float SkilCoolDownTime { get { return skilCoolDownTime; } }
    public float CurExp { get { return curExp; } set { curExp = value; } }
    public float MaxExp { get { return maxExp; } set { maxExp = value; } }

    public void Init()
    {
        level = initLevel;
        damage = initDamage;
        maxHp = initHp;
        curExp = 0;
        maxExp = initMaxExp;
    }
}
