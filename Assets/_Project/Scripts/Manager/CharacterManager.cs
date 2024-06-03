using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public Character[] characters;
    public bool[] isDeads;
    public bool isAllDead;
    public Transform leaderTr { get; set; }

    private void Awake()
    {
        if(instance == null )
        {
            instance = this;
        }
        isDeads = new bool[characters.Length];
    }
    public void Init()
    {
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i].transform.position = new Vector2((float)-i, -2f);
            characters[i].Init();
        }
    }
    public bool CheckAllDead()
    {
        for(int i = 0; i < isDeads.Length; i++)
        {
            if (isDeads[i].Equals(false)) return false;
        }
        isAllDead = true;
        return true;
    }
    public Transform GetFollowTr(int index)
    {
        return characters[index].transform;
    }
}
