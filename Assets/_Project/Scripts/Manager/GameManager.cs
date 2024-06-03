using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MonsterSpawner monsterSpawner;
    public CharacterManager characterManager;
    public UIManager UIManager;
    public bool isPlay = false;
    public bool isClear = false;
    public bool isFail = false;
    public int coin = 0;
    WaitForSeconds waitForSeconds;
    private void Awake()
    {
        if(instance == null) instance = this;
        Application.targetFrameRate = 30;
        waitForSeconds = new WaitForSeconds(3f);
        StopGame();
    }
    
    public void InitGame()
    {
        characterManager.Init();
        monsterSpawner.StartStage();
        
    }
    public void ClearGame()
    {
        isClear = true;
        monsterSpawner.EndStage();
        StartCoroutine(ClearUISetActive());
    }
    IEnumerator ClearUISetActive()
    {
        yield return waitForSeconds;
        UIManager.ClearGameUI.SetActive(true);
        UIManager.PlayGameUI.SetActive(false);
    }
    public void NextGame()
    {
        characterManager.Init();
        UIManager.PlayGameUI.SetActive(true);
        monsterSpawner.NextStage();
    }
    public void FailGame()
    {
        isFail = true;
        UIManager.FailGameUI.SetActive(true);
        UIManager.PlayGameUI.SetActive(false);
        //StopGame();
    }
    public void StopGame()
    {
        isPlay = false;
        Time.timeScale = 0;
    }
    
    public void StartGame()
    {
        isPlay = true;
        Time.timeScale = 1;
        InitGame();
    }
}
