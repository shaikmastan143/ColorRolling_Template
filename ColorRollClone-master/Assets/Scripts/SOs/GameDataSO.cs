using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR
using UnityEngine;

[CreateAssetMenu(fileName ="GameData", menuName ="ScriptableObjects/GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public int CurrentLevel { get; set; }
    public int HintNum { get; set; }
    public int CurrentRolledOutCount { get; set; }
    public int RolledOutUptoHintCount { get; set; }
    public int FirstTime { get; set; }

    public LevelSO[] levelSOs;



    public void ResetOnNextLevel()
    {
        //HintNum = 0;
        CurrentRolledOutCount = 0;
        RolledOutUptoHintCount = 0;
    }
    public void InitializeDefaultValues()
    {
        FirstTime = 0;
        CurrentLevel = 0;
        HintNum = 3;
        CurrentRolledOutCount = 0;
        RolledOutUptoHintCount = 0;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(GameDataSO))]
public class GameDataSOCE: Editor
{
    GameDataSO gameDataSO { get { return target as GameDataSO; } }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Reset"))
        {
            gameDataSO.ResetOnNextLevel();
            
            gameDataSO.CurrentLevel = 0;

            gameDataSO.HintNum = GlobalAccess.Current.ConstantsSO.DefaultHintNum;

            EditorUtility.SetDirty(gameDataSO);
        }

        base.OnInspectorGUI();

    }
}
#endif //UNITY_EDITOR
