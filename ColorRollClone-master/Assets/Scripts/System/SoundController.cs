using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    [SerializeField] private SettingsSO settingsSO;

    public bool VibrationEnabled { get => settingsSO.VibrationEnabled; set => settingsSO.VibrationEnabled = value; }
    public bool SoundEnabled { get => settingsSO.SoundEnabled; set => settingsSO.SoundEnabled = value; }

    [SerializeField] private AudioClip audioClipApplause;
    [SerializeField] private AudioClip audioClipPaperFirework;
    [SerializeField] private AudioClip audioClipButton;
    [SerializeField] private AudioClip audioClipCorrect;
    [SerializeField] private AudioClip audioClipCarpetRoll;
    [SerializeField] private AudioClip audioClipCarpetRollIn;
    [SerializeField] private AudioClip audioClipPopup;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Vibrate()
    {
        if (VibrationEnabled)
        {
            //Handheld.Vibrate();
            Vibration.Vibrate(75);
        }
    }

    public void PlayPaperFireworkSound()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipApplause);
            audioSource.PlayOneShot(audioClipPaperFirework);
        }
    }


    public void PlayButtonSound()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipButton);
        }
    }

    public void PlayCorrectSound()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipCorrect);
        }
    }

    public void PlayCarpetRollSound()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipCarpetRoll);
        }
    }


    public void PlayCarpetRollInSound()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipCarpetRollIn);
        }
    }
    public void PlayPopup()
    {
        if (SoundEnabled)
        {
            audioSource.PlayOneShot(audioClipPopup);
        }
    }
}

