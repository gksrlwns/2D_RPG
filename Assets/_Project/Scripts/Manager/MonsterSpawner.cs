using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 생성 Info")]
    public int stageLevel = 1;
    [SerializeField] float spawnTime = 5f;
    [SerializeField] int maxSpawnCount = 10;
    [SerializeField] int curSpawnCount = 0;
    [SerializeField] int concurrentSpawnCount = 5;
    public int dieCount = 0;
    [SerializeField] Bounds spawnPointBounds;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] Transform bossSpawnPos;
    [SerializeField] bool isBossSpawn;
    [Header("몬스터 Data")]
    [SerializeField] MonsterData monsterData;
    [SerializeField] List<Monster> monsters;
    float timer = 0f;

    private void Awake()
    {
        monsters = new List<Monster>();
    }

    private void Update()
    {
        if (isBossSpawn) return;
        timer += Time.deltaTime;
        if (dieCount >= maxSpawnCount)
        {
            BossSpawn();
            return;
        }
        if (curSpawnCount >= maxSpawnCount) return;
        
        if(timer >=spawnTime)
        {
            Spawn();
            timer = 0f;
        }
    }
    public void StartStage()
    {
        GameManager.instance.isClear = false;
        Init();
        for (int i = 0; i < concurrentSpawnCount; i++)
        {
            Spawn();
        }
    }
    public void EndStage()
    {
        stageLevel++;
        StageLevelUp();
    }
    public void NextStage()
    {
        StartStage();
    }
    public void Init()
    {
        for(int i = 0; i < monsters.Count; i++)
        {
            monsters[i].ForceReturnObject();
        }
        curSpawnCount = 0;
        dieCount = 0;
        isBossSpawn = false;
        timer = 0f;
        monsters.Clear();
    }
    void BossSpawn()
    {
        var bossMonster = Instantiate(bossPrefab).GetComponent<Monster>();
        bossMonster.InitBoss(monsterData);
        bossMonster.transform.position = bossSpawnPos.position;
        isBossSpawn = true;
    }
    void Spawn()
    {
        var monster = PoolManager.instance.GetObject(PoolType.Monster).GetComponent<Monster>();
        monsters.Add(monster);
        monster.Init(monsterData);
        monster.transform.position = SpawnPoint();
        curSpawnCount++;
    }
    public void StageLevelUp()
    {
        monsterData.maxHp *= Mathf.Pow(1.1f, stageLevel - 1);//monsterData.maxHp * stageLevel * 0.1f;
        monsterData.damage *= Mathf.Pow(1.1f, stageLevel - 1);
    }
    Vector2 SpawnPoint()
    {
        float randomPointX = transform.position.x + spawnPointBounds.center.x + UnityEngine.Random.Range(spawnPointBounds.extents.x * -0.5f, spawnPointBounds.extents.x * 0.5f);
        float randomPointY = transform.position.y + spawnPointBounds.center.y + UnityEngine.Random.Range(spawnPointBounds.extents.y * -0.5f, spawnPointBounds.extents.y * 0.5f);

        Vector2 spawnPos = new Vector2(randomPointX, randomPointY);
        return spawnPos;
    }

    private void OnDrawGizmosSelected()
    {
        Color color = Color.blue;
        color.b = 0.8f;
        color.a = 0.3f;
        Gizmos.color = color;

        Gizmos.DrawCube(spawnPointBounds.center, spawnPointBounds.size);
    }
}
[Serializable]
public class MonsterData
{
    public float maxHp = 100;
    public float damage = 10;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
}
