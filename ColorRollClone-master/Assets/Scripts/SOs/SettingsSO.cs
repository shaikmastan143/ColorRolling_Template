using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SettingsSO",menuName ="ScriptableObjects/SettingsSO")]
public class SettingsSO : ScriptableObject
{
    public bool VibrationEnabled;
    public bool SoundEnabled;
}
