using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StoreManager : MonoBehaviour
{
    [SerializeField] CharacterManager characterManager;

    [SerializeField] TMP_Text required_Damage_CoinText;
    [SerializeField] TMP_Text required_HP_CoinText;
    
    [SerializeField] int required_HP_Coin = 1;
    [SerializeField] int required_Damage_Coin = 1;

    private void Update()
    {
        required_Damage_CoinText.text = required_Damage_Coin.ToString();
        required_HP_CoinText.text = required_HP_Coin.ToString();
        
    }
    public void BuyDamage()
    {
        if (GameManager.instance.coin < required_Damage_Coin)
        {
            Debug.Log("돈이 부족합니다.");
        }
        else
        {
            DamageUp();
            GameManager.instance.coin -= required_Damage_Coin;
            required_Damage_Coin *= 2;
        }
    }
    public void BuyHP()
    {
        if (GameManager.instance.coin < required_HP_Coin)
        {
            Debug.Log("돈이 부족합니다.");
        }
        else
        {
            HPUP();
            GameManager.instance.coin -= required_HP_Coin;
            required_HP_Coin *= 2;
        }
    }
    void DamageUp()
    {
        for(int i = 0; i < characterManager.characters.Length; i++)
        {
            characterManager.characters[i].characterData.Damage += 1;
        }
    }
    void HPUP()
    {
        for (int i = 0; i < characterManager.characters.Length; i++)
        {
            characterManager.characters[i].characterData.MaxHp += 10;
        }
    }
}
