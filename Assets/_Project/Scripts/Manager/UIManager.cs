using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startGameUI;
    public GameObject ClearGameUI;
    public GameObject FailGameUI;
    public GameObject PlayGameUI;
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text stageText;
    
    private void Update()
    {
        coinText.text = GameManager.instance.coin.ToString();
        stageText.text = GameManager.instance.monsterSpawner.stageLevel.ToString();
    }
}
