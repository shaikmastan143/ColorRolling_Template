using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;

    public GameDataSO GameDataSO { get => gameDataSO; set => gameDataSO = value; }

    public Action<int> LevelChanged = delegate { };
    public Action<int> HintNumChanged = delegate { };

    public bool IsLevelValid()
    {
        return gameDataSO.CurrentLevel < gameDataSO.levelSOs.Length;
    }

    public bool IncreaseLevel()
    {
        if (gameDataSO.CurrentLevel < gameDataSO.levelSOs.Length)
        {
            gameDataSO.CurrentLevel++;

            LevelChanged?.Invoke(gameDataSO.CurrentLevel);

            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void DecreaseHintNum()
    {
        if (gameDataSO.HintNum >0)
        {
            gameDataSO.HintNum--;

            HintNumChanged?.Invoke(gameDataSO.HintNum);
        }
    }

    public void SetUsedHintCount(int n)
    {
        gameDataSO.HintNum = n;

        HintNumChanged?.Invoke(gameDataSO.HintNum);
    }
}
