using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLeftUI : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private MainGame mainGame;
    [SerializeField] private SettingsUI settings;

    void Start()
    {
        mainGame.LevelCompleted += HandleLevelCompleted;
        mainGame.NewLevelLoaded += HandleNewLevelLoaded;

        levelProgress.Init(mainGame.CurrentLevelInStage, mainGame.LevelNumInStage);
    }

    public void HandleOnSettingButtonClick()
    {
        settings.opener.Show();
    }

    private void HandleLevelCompleted(int level)
    {
        levelProgress.Init(mainGame.CurrentLevelInStage+1, mainGame.LevelNumInStage);
    }

    private void HandleNewLevelLoaded(int level)
    {
        levelProgress.Init(mainGame.CurrentLevelInStage, mainGame.LevelNumInStage);
    }
}
