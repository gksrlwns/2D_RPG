using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem stunEffect;
    private void Awake()
    {
        stunEffect.Stop();
    }
    public void StunEffect()
    {
        stunEffect.Play();
    }
}
