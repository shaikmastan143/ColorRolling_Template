using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopRightUI : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private MainGame mainGame;

    void Start()
    {
        mainGame.NewLevelLoaded += HandleLevelCompleted;

        levelText.text = (mainGame.gameDataManager.GameDataSO.CurrentLevel+1).ToString();
    }

    private void HandleLevelCompleted(int level)
    {
        levelText.text = (level+1).ToString();
    }
}
