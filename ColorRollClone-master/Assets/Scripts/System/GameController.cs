using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MainUI mainUI;
    [SerializeField] LevelCompleteUI stageCompletedUI;
    [SerializeField] MainGame mainGame;
    [SerializeField] AdvertisementController advertisementController;

    private void OnEnable()
    {
        WireUpEvents();
    }

    private void OnDisable()
    {
        UnwireEvents();
    }

    private void WireUpEvents()
    {
        mainUI.HintButtonClicked += mainGame.UseHint;

        mainGame.ShouldWatchAdsOnLevelCompleted += advertisementController.ShowInterstitialAd;

        advertisementController.UnityAdsDidFinish += HandleRewardedAdsFinished;

        mainUI.hintUI.AdsButtonClicked += advertisementController.ShowRewardedVideo;

        stageCompletedUI.PlayButtonClicked += advertisementController.ShowRewardedVideo;

        stageCompletedUI.Done += mainGame.LoadNextLevel;

        mainGame.StageCompleted += stageCompletedUI.HandleLevelCompleted;
    }

    private void UnwireEvents()
    {
        mainUI.HintButtonClicked -= mainGame.UseHint;

        mainGame.ShouldWatchAdsOnLevelCompleted -= advertisementController.ShowInterstitialAd;

        advertisementController.UnityAdsDidFinish -= HandleRewardedAdsFinished;

        mainUI.hintUI.AdsButtonClicked -= advertisementController.ShowRewardedVideo;

        stageCompletedUI.PlayButtonClicked -= advertisementController.ShowRewardedVideo;

        stageCompletedUI.Done -= mainGame.LoadNextLevel;

        mainGame.StageCompleted -= stageCompletedUI.HandleLevelCompleted;
    }

    private void HandleRewardedAdsFinished()
    {
        mainGame.IncreaseHintNumOnRewarded();

        mainUI.SpawnHintParticle(GlobalAccess.Current.ConstantsSO.DefaultHintNum);
    }
}
