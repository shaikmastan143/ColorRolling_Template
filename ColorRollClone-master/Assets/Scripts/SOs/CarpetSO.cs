using SkiaDemo1;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

using UnityEngine;

[CreateAssetMenu(fileName ="CarpetSO",menuName ="ScriptableObjects/CarpetSO", order =1)]
public class CarpetSO : ScriptableObject
{
    public List<Vector2> Polygon = new List<Vector2>();
    public Color Color = new Color(0,0,0,1);
    public Vector2 Position;
    public Vector2 PoleOfInaccessibility = new Vector2(0,0);
    public Vector2[] PivotPoints = new Vector2[2];
    public int Order = 0;
    public bool IsPivotClockwise = true;

    public bool stickPivotToEdge = true;
    public int pivotPoint1 = 0;
    public int pivotPoint2 = 1;

    //temp
    public bool displayDefaultEditor = false;
    public bool show = true;
}
#if UNITY_EDITOR

[CustomEditor(typeof(CarpetSO))]
public class CarpetSOCE : Editor
{
    CarpetSO carpetSO { get { return (CarpetSO)target; } }

    //passed in variables
    public int index;
    public int carpetNum;
    //public bool isDisplayedFromGameEditor = false;
    
    //caching variables
    private string[] orderOptions;

    public void SetCarpetNum(int carpetNum)
    {
        orderOptions = new string[carpetNum];
        for (int i = 0; i < carpetNum; i++)
            orderOptions[i] = i.ToString();
    }

    public override void OnInspectorGUI()
    {
        carpetSO.displayDefaultEditor = EditorGUILayout.Toggle("Display Default Editor", carpetSO.displayDefaultEditor);
        if (carpetSO.displayDefaultEditor)
        {
            base.OnInspectorGUI();
            return;
        }
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        carpetSO.show =EditorGUILayout.Foldout(carpetSO.show, "Carpet " + carpetSO.Order, true, EditorStyles.foldout);
        carpetSO.Color = EditorGUILayout.ColorField(carpetSO.Color);
        EditorGUILayout.EndHorizontal();

        if (carpetSO.show)
        {
            drawCarpet();
        }

        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(target);

        //DrawDefaultInspector();
        //serializedObject.ApplyModifiedProperties();
    }

    private void drawCarpet()
    {
        EditorGUI.indentLevel++;
        //if (orderOptions != null)
        //{
        //    var selectedOrder = orderOptions[EditorGUILayout.Popup("Order", Mathf.Min(carpetSO.Order, orderOptions.Length - 1), orderOptions)];
        //    if (selectedOrder != null)
        //        carpetSO.Order = int.Parse(selectedOrder);
        //}

        EditorGUILayout.LabelField("Vertices " + carpetSO.Polygon.Count);

        int optionNum = Mathf.Max(0, carpetSO.Polygon.Count - 1);
        
        string[] pivotOptions1 = new string[optionNum];
        string[] pivotOptions2 = new string[optionNum];

        int temp1 = 0, temp2 = 0, optionIndex1 = 0, optionIndex2 = 0, left = 0, right = 0;

        for (int i = 0; i < carpetSO.Polygon.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("" + i, GUILayout.Width(40));
            carpetSO.Polygon[i] = EditorGUILayout.Vector2Field("", carpetSO.Polygon[i]);
            if (GUILayout.Button("Del"))
                carpetSO.Polygon.RemoveAt(i);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();

            if (carpetSO.stickPivotToEdge && optionNum > 0)
            {
                if (i == carpetSO.pivotPoint1)
                {
                    carpetSO.PivotPoints[0] = carpetSO.Polygon[i];

                    optionIndex1 = temp1;
                }

                if (i == carpetSO.pivotPoint2)
                {
                    carpetSO.PivotPoints[1] = carpetSO.Polygon[i];

                    optionIndex2 = temp2;
                }

                if (i != carpetSO.pivotPoint2)
                {
                    pivotOptions1[temp1++] = i.ToString();
                }

                if (i != carpetSO.pivotPoint1)
                {
                    pivotOptions2[temp2++] = i.ToString();
                }
            }
        }

        for (int i = 0; i < carpetSO.Polygon.Count; i++)
        {
            var dot = Utils.Instance.LeftOrRight(carpetSO.PivotPoints[1] - carpetSO.PivotPoints[0], carpetSO.Polygon[i] - carpetSO.PivotPoints[0]);

            if (dot > 0)
            {
                right++;
            }
            else if (dot < 0)
            {
                left++;
            }
        }
        carpetSO.IsPivotClockwise = (right >= left);

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Reorder"))
        {
            int n = carpetSO.Polygon.Count;
            var p = carpetSO.Polygon;
            float sum = 0;
            for (int i = 0; i <n; i++)
            {
                int i2 = (i == n - 1 ? 0 : i+1);
                sum += (p[i2].x - p[i].x) * (p[i2].y + p[i].y);
            }
            if (sum < 0)
            {
                //counter-clockwise

                var newPolygon = new List<Vector2>();
                for (int i = n-1; i >=0 ; i--)
                {
                    newPolygon.Add(p[i]);
                }
                carpetSO.Polygon = newPolygon;
            }
        }
        if (GUILayout.Button("Round up"))
        {
            carpetSO.Position = new Vector2(Mathf.Round(carpetSO.Position.x), Mathf.Round(carpetSO.Position.y));

            for (int i = 0; i < carpetSO.Polygon.Count; i++)
            {
                carpetSO.Polygon[i] = new Vector2(Mathf.Round(carpetSO.Polygon[i].x), Mathf.Round(carpetSO.Polygon[i].y));
            }
        }
        if (GUILayout.Button("New Vertex"))
        {
            Vector2 newVertex = new Vector2(0, 0);

            if (carpetSO.Polygon.Count > 1)
            {
                newVertex = (carpetSO.Polygon[0] + carpetSO.Polygon[carpetSO.Polygon.Count - 1]) * 0.5f;
            }

            carpetSO.Polygon.Add(new Vector2(Mathf.Round(newVertex.x), Mathf.Round(newVertex.y)));

        }
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.LabelField("Pivot Edge ");
        EditorGUI.indentLevel++;
        carpetSO.stickPivotToEdge = EditorGUILayout.Toggle("Stick Pivot to Edge", carpetSO.stickPivotToEdge);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(carpetSO.stickPivotToEdge ? ("Pivot 1 -> Vertex " + carpetSO.pivotPoint1) : "Pivot 1 ", GUILayout.Width(140));
        if (carpetSO.stickPivotToEdge && optionNum > 0)
        {
            var selected = pivotOptions1[EditorGUILayout.Popup("", optionIndex1, pivotOptions1, GUILayout.Width(60))];
            if (selected != null)
            {
                carpetSO.pivotPoint1 = int.Parse(selected);
            }
        }
        EditorGUI.BeginDisabledGroup(carpetSO.stickPivotToEdge);
        Vector2 newPivot1 = EditorGUILayout.Vector2Field("", carpetSO.PivotPoints[0]);
        EditorGUI.EndDisabledGroup();
        carpetSO.PivotPoints[0] = carpetSO.stickPivotToEdge ? newPivot1 : carpetSO.PivotPoints[0];
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(carpetSO.stickPivotToEdge ? ("Pivot 2 -> Vertex " + carpetSO.pivotPoint2) : "Pivot 2 ", GUILayout.Width(140));
        if (carpetSO.stickPivotToEdge && optionNum > 0)
        {
            var selected = pivotOptions2[EditorGUILayout.Popup("", optionIndex2, pivotOptions2, GUILayout.Width(60))];
            if (selected != null)
                carpetSO.pivotPoint2 = int.Parse(selected);
        }
        EditorGUI.BeginDisabledGroup(carpetSO.stickPivotToEdge);
        Vector2 newPivot2 = EditorGUILayout.Vector2Field("", carpetSO.PivotPoints[1]);
        EditorGUI.EndDisabledGroup();
        carpetSO.PivotPoints[1] = carpetSO.stickPivotToEdge ? newPivot2 : carpetSO.PivotPoints[1];
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;

        GUILayout.Space(10);

        carpetSO.Position = EditorGUILayout.Vector2Field("Position", carpetSO.Position);

        GUILayout.Space(10);

        EditorGUI.indentLevel--;



    }
}
#endif //UNITY_EDITOR
