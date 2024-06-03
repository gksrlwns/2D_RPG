using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public float damage;
    public GameObject target;
    public void SeekAndDamage(float _damage,GameObject _target)
    {
        target = _target;
        damage = _damage;
    }
    
    public void MeleeAttack()
    {
        Character character = target.GetComponent<Character>();
        if (!character.isLive) return;
        character.Damaged(damage);
    }
}
