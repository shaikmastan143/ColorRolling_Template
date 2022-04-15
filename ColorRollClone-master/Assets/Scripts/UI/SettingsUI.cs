using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [NonSerialized] public CongratulationUI opener;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle soundToggle;

    private void Awake()
    {
        opener = GetComponent<CongratulationUI>();

        vibrationToggle.isOn = SoundController.Current.VibrationEnabled;
        soundToggle.isOn = SoundController.Current.SoundEnabled;
    }

    public void HandleCloseButtonClicked()
    {
        opener.Hide();

    }

    public void HandleShareButtonClicked()
    {
        Share();
    }

    public void HandleRatingButtonClicked()
    {
        Rating();
    }

    public void HandleVibrationToggle(bool value)
    {
        SoundController.Current.VibrationEnabled = vibrationToggle.isOn;
    }

    public void HandleSoundToggle(bool value)
    {
        SoundController.Current.SoundEnabled = soundToggle.isOn;
    }


    public void Rating()
    {
        Application.OpenURL(GlobalAccess.Current.ConstantsSO.GameURL);
    }
    public void Share()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Collor Roll Game").SetText("Check out for the game!")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
