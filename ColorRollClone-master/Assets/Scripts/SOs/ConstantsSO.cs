using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Constants",menuName ="ScriptableObjects/ConstantsSO")]
public class ConstantsSO : ScriptableObject
{
    public int DefaultHintNum = 3;
    public int LevelNumPerAd = 3;
    public string GameURL = "www.google.com";
}
