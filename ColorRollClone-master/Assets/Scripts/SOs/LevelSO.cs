using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR
using UnityEngine;

public class LevelSO : ScriptableObject
{
    [SerializeField]
    public List<CarpetSO> carpetSOs = new List<CarpetSO>();
    public Vector2 Position = new Vector2();
}
#if UNITY_EDITOR

[CustomEditor(typeof(LevelSO))]
[CanEditMultipleObjects]
public class LevelSOCE : Editor
{
    LevelSO levelSO { get { return (LevelSO)target; } }

    public override void OnInspectorGUI()
    {
        if (levelSO == null) return;

        levelSO.Position = EditorGUILayout.Vector2Field("Position", levelSO.Position);

        levelSO.carpetSOs.Sort(new CarpetSOComparer());
        for (int i = 0; i < levelSO.carpetSOs.Count; i++)
        {
            if (levelSO.carpetSOs[i] == null) continue;

            var carpetSOCE = Editor.CreateEditor(levelSO.carpetSOs[i]) as CarpetSOCE;
            carpetSOCE.index = i;
            //carpetSOCE.isDisplayedFromGameEditor = true;
            carpetSOCE.SetCarpetNum(levelSO.carpetSOs.Count);
            carpetSOCE.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move Up"))
            {
                if (i > 0)
                {
                    levelSO.carpetSOs[i].Order--;
                    levelSO.carpetSOs[i - 1].Order++;
                    var temp = levelSO.carpetSOs[i];
                    levelSO.carpetSOs[i] = levelSO.carpetSOs[i - 1];
                    levelSO.carpetSOs[i - 1] = temp;
                }
            }
            if (GUILayout.Button("Move Down"))
            {
                if (i < levelSO.carpetSOs.Count - 1)
                {
                    levelSO.carpetSOs[i].Order++;
                    levelSO.carpetSOs[i + 1].Order--;
                    var temp = levelSO.carpetSOs[i];
                    levelSO.carpetSOs[i] = levelSO.carpetSOs[i + 1];
                    levelSO.carpetSOs[i + 1] = temp;
                }
            }
            if (GUILayout.Button("Duplicate"))
            {
                var newCarpetSO = Utils.Instance.CreateAsset<CarpetSO>("Assets/SO Data/Carpets/Carpet.asset");
                newCarpetSO.Polygon = new List<Vector2>(levelSO.carpetSOs[i].Polygon);
                newCarpetSO.Color = levelSO.carpetSOs[i].Color;
                newCarpetSO.Position = levelSO.carpetSOs[i].Position;
                newCarpetSO.PivotPoints = (Vector2[])levelSO.carpetSOs[i].PivotPoints.Clone();
                newCarpetSO.Order = levelSO.carpetSOs.Count;
                newCarpetSO.IsPivotClockwise = levelSO.carpetSOs[i].IsPivotClockwise;
                newCarpetSO.stickPivotToEdge = levelSO.carpetSOs[i].stickPivotToEdge;
                newCarpetSO.pivotPoint1= levelSO.carpetSOs[i].pivotPoint1;
                newCarpetSO.pivotPoint2 = levelSO.carpetSOs[i].pivotPoint2;
                levelSO.carpetSOs.Add(newCarpetSO);
            }
            if (GUILayout.Button("Remove"))
            {
                Utils.Instance.DeleteScriptableObject(levelSO.carpetSOs[i]);
                levelSO.carpetSOs.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20);

        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Carpet"))
        {
            var newCarpetSO = Utils.Instance.CreateAsset<CarpetSO>("Assets/SO Data/Carpets/Carpet.asset");
            newCarpetSO.Order = levelSO.carpetSOs.Count;
            levelSO.carpetSOs.Add(newCarpetSO);
        }
        if (GUILayout.Button("Triangle"))
        {
            var newCarpetSO = Utils.Instance.CreateAsset<CarpetSO>("Assets/SO Data/Carpets/Carpet.asset");
            newCarpetSO.Order = levelSO.carpetSOs.Count;

            newCarpetSO.Polygon.Add(new Vector2(-1,-1));
            newCarpetSO.Polygon.Add(new Vector2(0,1));
            newCarpetSO.Polygon.Add(new Vector2(1,-1));

            levelSO.carpetSOs.Add(newCarpetSO);
        }
        if (GUILayout.Button("Rectangle"))
        {
            var newCarpetSO = Utils.Instance.CreateAsset<CarpetSO>("Assets/SO Data/Carpets/Carpet.asset");
            newCarpetSO.Order = levelSO.carpetSOs.Count;

            newCarpetSO.Polygon.Add(new Vector2(-1, -1));
            newCarpetSO.Polygon.Add(new Vector2(-1, 1));
            newCarpetSO.Polygon.Add(new Vector2(1, 1));
            newCarpetSO.Polygon.Add(new Vector2(1, -1));

            levelSO.carpetSOs.Add(newCarpetSO);
        }

        EditorGUILayout.EndHorizontal();

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
#endif //UNITY_EDITOR
