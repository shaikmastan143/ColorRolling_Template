using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameEditor : MonoBehaviour
{
    [SerializeField] private GameDataSO GameDataSO;
    
    public LevelSO levelSO;
    

    public void CreateNewLevelSO()
    {

#if UNITY_EDITOR
        levelSO = Utils.Instance.CreateAsset<LevelSO>("Assets/SO Data/Levels/Level.asset");

        var newArray = new LevelSO[GameDataSO.levelSOs.Length + 1];

        GameDataSO.levelSOs.CopyTo(newArray, 0);

        newArray[GameDataSO.levelSOs.Length] = levelSO;

        GameDataSO.levelSOs = newArray;

#endif //UNITY_EDITOR

    }

}
#if UNITY_EDITOR

[CustomEditor(typeof(GameEditor))]
[CanEditMultipleObjects]
public class GameEditorCE : Editor
{
    GameEditor gameEditor { get { return (GameEditor)target; } }
    private Vector3[] polygon3D = new Vector3[0];
    private Editor levelSOCE = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (gameEditor.levelSO != null)
        {
            if (levelSOCE == null || levelSOCE.target != gameEditor.levelSO)
            {
                levelSOCE = Editor.CreateEditor(gameEditor.levelSO);
                gameEditor.transform.position = new Vector3(gameEditor.levelSO.Position.x,0, gameEditor.levelSO.Position.y);
            }
        }
        levelSOCE?.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("New Level"))
        {
            gameEditor.CreateNewLevelSO();
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(target);

    }
    private void OnSceneGUI()
    {
        if (gameEditor.levelSO == null) return;

        gameEditor.levelSO.Position = new Vector2(gameEditor.transform.position.x, gameEditor.transform.position.z);

        for (int i = 0; i < gameEditor.levelSO.carpetSOs.Count; i++)
        {
            var carpetSO = gameEditor.levelSO.carpetSOs[i];
            if (carpetSO == null) continue;
            var polygon = carpetSO.Polygon;
            int n = polygon.Count;
            var origin = new Vector3(carpetSO.Position.x, 0, carpetSO.Position.y);
            if (polygon3D.Length != n) polygon3D = new Vector3[n];
            for (int j = 0; j < n; j++)
            {
                var v = polygon[j];
                var p = gameEditor.transform.TransformPoint(origin + new Vector3(v.x, 0, v.y));
                if (carpetSO.show)
                {
                    float size = HandleUtility.GetHandleSize(p) * 0.3f;
                    var snap = Vector3.one*0.5f;
                    var handlePositionInWorldSpace = Handles.FreeMoveHandle(p, Quaternion.identity, size, snap, Handles.SphereHandleCap);
                    var newPos = gameEditor.transform.InverseTransformPoint(handlePositionInWorldSpace) - origin;
                    carpetSO.Polygon[j] = new Vector2(newPos.x, newPos.z);
                }

                //var v2 = polygon[j == n - 1 ? 0 : j + 1];
                //Vector3 p2 = gameEditor.transform.TransformPoint(origin+new Vector3(v2.x, 0, v2.y));
                //Handles.DrawLine(p, p2);

                polygon3D[j] = p;
            }
            Handles.color = carpetSO.Color;// new Color(carpetSO.Color.r, carpetSO.Color.g, carpetSO.Color.b,0.5f);
            Handles.DrawAAConvexPolygon(polygon3D);

            Handles.color = Color.red;

            var pivotPoints = carpetSO.PivotPoints;
            Vector3 pivot1 = gameEditor.transform.TransformPoint(origin + new Vector3(pivotPoints[0].x, 0, pivotPoints[0].y));
            Vector3 pivot2 = gameEditor.transform.TransformPoint(origin + new Vector3(pivotPoints[1].x, 0, pivotPoints[1].y));
            if (!carpetSO.stickPivotToEdge)
            {
                float size = HandleUtility.GetHandleSize(pivot1) * 0.3f;
                var snap = Vector3.one * 0.5f;
                Vector3 worldSpacePivot1 = Handles.FreeMoveHandle(pivot1, Quaternion.identity, size, snap, Handles.SphereHandleCap);
                Vector3 worldSpacePivot2 = Handles.FreeMoveHandle(pivot2, Quaternion.identity, size, snap, Handles.SphereHandleCap);
                var pivot13D = gameEditor.transform.InverseTransformPoint(worldSpacePivot1) - origin;
                var pivot23D = gameEditor.transform.InverseTransformPoint(worldSpacePivot2) - origin;
                pivotPoints[0] = new Vector2(pivot13D.x, pivot13D.z);
                pivotPoints[1] = new Vector2(pivot23D.x, pivot23D.z);
            }
            Handles.DrawLine(pivot1, pivot2);
            Handles.color = Color.white;

            var newPosition = gameEditor.transform.InverseTransformPoint(Handles.PositionHandle(gameEditor.transform.TransformPoint(new Vector3(carpetSO.Position.x, 0, carpetSO.Position.y)), Quaternion.identity));
            carpetSO.Position = new Vector2(newPosition.x, newPosition.z);

            //EditorUtility.SetDirty(gameEditor.levelSO.carpetSOs[i]);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);

    }
}
#endif //UNITY_EDITOR
