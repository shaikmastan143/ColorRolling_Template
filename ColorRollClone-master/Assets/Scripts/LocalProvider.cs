using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalProvider : Singleton<LocalProvider>
{
    [SerializeField] private GameDataSO gameDataSO;

    private void Awake()
    {

        DontDestroyOnLoad(this);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.runInBackground = false;

    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("Pause "+ pause);
        if (pause)
        {
            Save();
        }
        else
        {
            Load();
        }
    }


    public void Save()
    {
        Debug.Log("Saved");

        PlayerPrefs.SetInt("FirstTime", gameDataSO.FirstTime);
        PlayerPrefs.SetInt("CurrentLevel", gameDataSO.CurrentLevel);
        PlayerPrefs.SetInt("HintNum", gameDataSO.HintNum);
        PlayerPrefs.SetInt("CurrentRolledOutCount", gameDataSO.CurrentRolledOutCount);
        PlayerPrefs.SetInt("RolledOutUptoHintCount", gameDataSO.RolledOutUptoHintCount);

        PlayerPrefs.Save();
    }


    public void Load()
    {
        Debug.Log("Loaded");

        gameDataSO.FirstTime = PlayerPrefs.GetInt("FirstTime",1);

        if (gameDataSO.FirstTime == 1)
        {
            gameDataSO.InitializeDefaultValues();
        }
        else
        {
            gameDataSO.CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
            gameDataSO.HintNum = PlayerPrefs.GetInt("HintNum");
            gameDataSO.CurrentRolledOutCount = PlayerPrefs.GetInt("CurrentRolledOutCount");
            gameDataSO.RolledOutUptoHintCount = PlayerPrefs.GetInt("RolledOutUptoHintCount");
        }
    }
}
