using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType { Projectile_Arrow, Projectile_EnergyVolt, Projectile_ArrowSkill, Monster }

public class PoolManager : MonoBehaviour
{
    [Serializable]
    public class PoolData
    {
        public PoolType poolType;
        public int size;
        public GameObject prefab;
    }
    public static PoolManager instance;
    public Queue<GameObject>[] poolQueues;
    [SerializeField] private PoolData[] poolDatas;
    [SerializeField] private List<Transform> poolDataTr = new List<Transform>();

    private void Awake()
    {
        if (instance == null) instance = this;
        poolQueues = new Queue<GameObject>[poolDatas.Length];
        for (int i = 0; i < poolQueues.Length; i++)
        {
            poolQueues[i] = new Queue<GameObject>();
            GameObject obj = new GameObject(poolDatas[i].poolType.ToString());
            obj.transform.parent = transform;
            poolDataTr.Add(obj.transform);

            for (int j = 0; j < poolDatas[i].size; j++)
            {
                CreateObject(poolDatas[i], poolQueues[i]);
            }
        }
    }
    #region QUEUE
    /// <summary>
    /// isParent는 ParentTransform이 필요한 Objcet의 경우(Player의 자식오브젝트로 들어가는 경우)가 있어 매개변수를 통해 관리
    /// </summary>
    /// <param name="pooldata"></param>
    /// <param name="queue"></param>
    /// <param name="isParent"></param>
    /// <returns></returns>
    private GameObject CreateObject(PoolData pooldata, Queue<GameObject> queue)
    {
        var obj = Instantiate(pooldata.prefab);
        obj.transform.parent = poolDataTr[(int)pooldata.poolType];
        obj.SetActive(false);
        queue.Enqueue(obj);

        return obj;
    }

    public GameObject GetObject(PoolType poolType)
    {
        GameObject obj = null;
        var queue = poolQueues[(int)poolType];

        if (queue.Count <= 0)
        {
            PoolData poolData = poolDatas[(int)poolType];
            CreateObject(poolData, queue);
        }

        obj = queue.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(PoolType poolType, GameObject gameObject)
    {
        gameObject.SetActive(false);
        poolQueues[(int)poolType].Enqueue(gameObject);
    }
    #endregion
}
