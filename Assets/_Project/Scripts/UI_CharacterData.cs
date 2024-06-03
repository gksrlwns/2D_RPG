using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CharacterData : MonoBehaviour
{
    [SerializeField] CharacterData characterData;
    [SerializeField] Character character;

    [SerializeField] TMP_Text level_Text;
    [SerializeField] TMP_Text exp_Text;
    [SerializeField] TMP_Text hp_Text;
    [SerializeField] TMP_Text damage_Text;

    private void Update()
    {
        level_Text.text = $"{characterData.Level}";
        exp_Text.text = $"{characterData.CurExp} / {characterData.MaxExp}";
        hp_Text.text = $"{Mathf.CeilToInt(character.curHp)} / {characterData.MaxHp}";
        damage_Text.text = $"{characterData.Damage}";
    }
}
