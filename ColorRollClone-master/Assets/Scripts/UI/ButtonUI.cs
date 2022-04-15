using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }
    public void OnButtonClicked()
    {
        SoundController.Current.PlayButtonSound();
    }
}
