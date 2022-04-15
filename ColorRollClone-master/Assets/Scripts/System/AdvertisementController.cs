using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementController : MonoBehaviour, IUnityAdsListener
{
#if UNITY_ANDROID
    private string gameId = "3874363";
#elif UNITY_IOS
    private string gameId = "3874362";
#endif

    public string placementIdRewardedVideo = "rewardedVideo";
    public string placementIdBanner = "banner";

    public Action UnityAdsDidFinish = delegate { };

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId);

        StartCoroutine(ShowBannerWhenInitialized());
    }

    public void ShowRewardedVideo()
    {
        if (Advertisement.IsReady(placementIdRewardedVideo))
        {
            Advertisement.Show(placementIdRewardedVideo);
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placementIdBanner);
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("OnUnityAdsReady");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("OnUnityAdsDidError");

    }

    public void OnUnityAdsDidStart(string placementId)
    {

        Debug.Log("OnUnityAdsDidStart");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId.Equals(placementIdRewardedVideo))
        {
            if(showResult == ShowResult.Finished)
            {
                UnityAdsDidFinish?.Invoke();
            }
        }
        else
        {

        }
    }
}
