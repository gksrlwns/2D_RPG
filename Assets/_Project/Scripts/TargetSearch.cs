using UnityEngine;

public class TargetSearch : MonoBehaviour
{
    [Header("TargetSearch Info")]
    [SerializeField] float searchRange = 5;
    //public RaycastHit2D[] hits;
    [SerializeField] Collider2D[] colliders;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask characterLayer;
    public GameObject target;
    public Transform[] targets;

    private void Update()
    {
        //hits = Physics2D.CircleCastAll(transform.position, searchRange, Vector2.zero, 0, targetLayer);
        colliders = Physics2D.OverlapCircleAll(transform.position, searchRange, targetLayer);
        if (colliders.Length != 0) target = NearTarget();
        else target = null;
    }

    GameObject NearTarget()
    {
        GameObject nearTarget = null;
        float shortestDistance = Mathf.Infinity;
        for (int i = 0; i< colliders.Length; i++)
        {
            float distanceToMonster = Vector2.Distance(transform.position, colliders[i].transform.position);
            if (distanceToMonster < shortestDistance)
            {
                shortestDistance = distanceToMonster;
                nearTarget = colliders[i].gameObject;
            }
        }

        if(nearTarget && shortestDistance <= searchRange)
        {
            return nearTarget;
        }
        else
        {
            return null;
        }
    }

    public Monster[] MonstersInRange(float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,range, targetLayer);
        Monster[] monsters = new Monster[colliders.Length];
        for(int i = 0; i< colliders.Length; i++)
        {
            monsters[i] = colliders[i].GetComponent<Monster>();
        }

        return monsters;
    }
    public Character[] CharactersInRange(float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, characterLayer);
        Character[] characters = new Character[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            characters[i] = colliders[i].GetComponent<Character>();
        }

        return characters;
    }
    //Transform NearTarget()
    //{
    //    Transform target = null;
    //    float shortestDistance = Mathf.Infinity;
    //    for(int i = 0; i < hits.Length; i++)
    //    {
    //        float distanceToMonster = Vector2.Distance(transform.position, hits[i].transform.position);
    //        if(distanceToMonster < shortestDistance)
    //        {
    //            shortestDistance = distanceToMonster;
    //            target = hits[i].transform;
    //        }
    //    }
    //    return target;
    //}


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
}
