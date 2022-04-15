using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    [SerializeField] GameObject adsButton;
    [SerializeField] Button hintButton;
    [SerializeField] Text hintButtonText;

    [NonSerialized] public int rememberedHintNum;

    public Action AdsButtonClicked = delegate { };

    public void ShakeHintButton()
    {
        var animator = hintButton.GetComponent<Animator>();

        animator?.SetTrigger("Shake");

        SoundController.Current.Vibrate();
    }

    public void UpdateHintButton(int hintNum)
    {
        rememberedHintNum = hintNum;

        if (hintNum > 0)
        {
            hintButtonText.text = $"Hint {Mathf.Max(0, hintNum)}";

            HideAdsButton();
        }
        else
        {
            hintButtonText.text = "Hint?";

            ShowAdsButton();
        }
    }

    public void ShowAdsButton()
    {
        adsButton.SetActive(true);
    }
    public void HideAdsButton()
    {
        adsButton.SetActive(false);
    }

    public void HandleAdsButtonClicked()
    {
        AdsButtonClicked?.Invoke();
    }
}
