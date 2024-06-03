using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CharacterEffect : MonoBehaviour
{
    [SerializeField] GameObject skillEffect;
    [SerializeField] GameObject healEffect;
    [SerializeField] GameObject levelUpEffect;
    [SerializeField] float skillDuration;
    ParticleSystem skillParticle;
    CharacterData characterData;
    WaitForSeconds waitForSeconds;
    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(skillDuration);
        characterData = GetComponent<Character>().characterData;
    }
    public void SkillEffect()
    {
        if (skillEffect.Equals(null)) return;
        switch (characterData.CharacterType)
        {
            case CharacterType.Knight:
                break;
            case CharacterType.Thief:
                skillEffect.SetActive(true);
                StartCoroutine(SkillOut());
                break;
        }
        
    }
    public void HealEffect()
    {
        healEffect.SetActive(true);
        StartCoroutine(HealOut());
    }
    public void LevelUpEffect()
    {
        levelUpEffect.SetActive(true);
        StartCoroutine(LevelUpOut());
    }
    IEnumerator SkillOut()
    {
        yield return waitForSeconds;
        skillEffect.SetActive(false);
    }
    IEnumerator HealOut()
    {
        yield return waitForSeconds;
        healEffect.SetActive(false);
    }
    IEnumerator LevelUpOut()
    {
        yield return waitForSeconds;
        levelUpEffect.SetActive(false);
    }
}
