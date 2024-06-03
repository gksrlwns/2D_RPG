using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Arrow,ArrowSkill, EnergyVolt }

    [SerializeField ]ProjectileType type;
    [SerializeField] Monster target;
    [SerializeField] Vector2 dir;
    [SerializeField] float speed = 3f;
    [SerializeField] float damage;
    [SerializeField] float targetDistance;
    [SerializeField] CharacterData characterData;
    WaitForSeconds waitForSeconds;
    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(2f);
    }

    private void Update()
    {
        Move();
        Rotate();
        if (dir.magnitude <= 0.1f)
        {
            switch (type)
            {
                case ProjectileType.Arrow:
                    PoolManager.instance.ReturnObject(PoolType.Projectile_Arrow, gameObject);
                    break;
                case ProjectileType.ArrowSkill:
                    PoolManager.instance.ReturnObject(PoolType.Projectile_ArrowSkill, gameObject);
                    break;
                case ProjectileType.EnergyVolt:
                    PoolManager.instance.ReturnObject(PoolType.Projectile_EnergyVolt, gameObject);
                    break;
            }
            target.Damaged(damage,characterData);
            return;
        }
    }
    private void OnEnable()
    {
        StartCoroutine(ReturnPool());
    }
    IEnumerator ReturnPool()
    {
        yield return waitForSeconds;
        switch (type)
        {
            case ProjectileType.Arrow:
                PoolManager.instance.ReturnObject(PoolType.Projectile_Arrow, gameObject);
                break;
            case ProjectileType.ArrowSkill:
                PoolManager.instance.ReturnObject(PoolType.Projectile_ArrowSkill, gameObject);
                break;
            case ProjectileType.EnergyVolt:
                PoolManager.instance.ReturnObject(PoolType.Projectile_EnergyVolt, gameObject);
                break;
        }
    }
    public void Seek(Monster _target)
    {
        target = _target;
    }
    public void Damage(float _damage)
    {
        damage = _damage;
    }
    public void CharacterData(CharacterData _characterData)
    {
        characterData = _characterData;
    }
    public void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
    public void Rotate()
    {
        dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    
}
