using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR
using UnityEngine;

public class Utils
{
    /*Singleton declaration*/
    private static Utils instance = null;
    public static Utils Instance
    {
        get
        {
            if (instance == null)
                instance = new Utils();
            return instance;
        }
    }
    Utils() { }
    /*End of singleton declaration*/

    public float Tolerant = 0.001f;

    public bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];

            if (PointOnLine(p, pi, pj))
            {
                return false;
            }

            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;// ? 1 : -1;
    }
    public bool ContainsPoint(List<Vector2> polyPoints, Vector2 p)
    {
        var j = polyPoints.Count - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Count; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (PointOnLine(p, pi, pj))
            {
                return false;
            }
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;//? 1 : -1;
    }
    public bool PointOnLine(Vector2 point, Vector2 l1, Vector2 l2)
    {
        Vector2 dp = point - l1;
        Vector2 dl = l2 - l1;
        float cross = dp.x * dl.y - dp.y * dl.x;
        return (Mathf.Abs(cross) <= 0.00001f);
    }
#if UNITY_EDITOR

    public T CreateAsset<T>(string path) where T : ScriptableObject
    {
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);// path + "/New " + typeof(T).ToString() + ".asset");

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        //Selection.activeObject = asset;

        return asset;
    }

    public void DeleteScriptableObject(ScriptableObject o)
    {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(o));
        ScriptableObject.DestroyImmediate(o);
    }
#endif //UNITY_EDITOR

    public float LeftOrRight(Vector2 a, Vector2 b)
    {
        var dot = a.x * -b.y + a.y * b.x;
        return dot;// >= 0;//right or parallel
    }

    public bool PolygonCollision(Vector2[] p1, Vector2[] p2, Vector2 offset1, Vector2 offset2, Vector2 POA1,Vector2 POA2)
    {
        for (int i = 0; i < p1.Length; i++)
        {
            var ei1 = p1[i] + offset1;
            var ei2 = p1[((i == p1.Length - 1) ? 0 : (i+1))] + offset1;

            for (int j = 0; j < p2.Length; j++)
            {
                var ej1 = p2[j] + offset2;
                var ej2 = p2[((j == p2.Length - 1) ? 0 : (j+1))] + offset2;
                if (CheckLineIntersection(ei1, ei2, ej1, ej2))
                {
                    return true;
                }
            }
        }

        if (ContainsPoint(p1, POA2+offset2)|| ContainsPoint(p2, POA1+offset1))
        {
            return true;
        }

        for (int i = 0; i < p2.Length; i++)
        {
            if (ContainsPoint(p1, p2[i] + offset2 - offset1))
            {
                return true;
            }
        }
        for (int i = 0; i < p1.Length; i++)
        {
            if (ContainsPoint(p2, p1[i] + offset1 - offset2))
            {
                return true;
            }
        }
        return false;
    }

    public bool PolygonCollision(List<Vector2> p1, List<Vector2> p2, Vector2 offset1, Vector2 offset2, Vector2 POA1, Vector2 POA2)
    {
        return PolygonCollision(p1.ToArray(), p2.ToArray(), offset1, offset2, POA1, POA2);
    }

    public bool CheckLineIntersection(Vector2 la1, Vector2 la2, Vector2 lb1, Vector2 lb2)
    {
        float x1 = la1.x; 
        float y1 = la1.y;

        float x2 = la2.x;
        float y2 = la2.y;

        float x3 = lb1.x;
        float y3 = lb1.y;

        float x4 = lb2.x;
        float y4 = lb2.y;

        float s = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        
        if (s == 0) return false;

        float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4))/s;
        float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3))/s;

        if ((t >= Tolerant && t < 1.0f- Tolerant) && (u > Tolerant && u < 1.0f- Tolerant))
        {
            return true;
        }

        return false;
    }

    public int CheckLineIntersection(Vector2 la1, Vector2 la2, Vector2 lb1, Vector2 lb2, out Vector2 intersection)
    {
        intersection = new Vector2();

        float x1 = la1.x;
        float y1 = la1.y;

        float x2 = la2.x;
        float y2 = la2.y;

        float x3 = lb1.x;
        float y3 = lb1.y;

        float x4 = lb2.x;
        float y4 = lb2.y;

        float s = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (s == 0)
        {
            if (Vector2.Dot(la2-la1,lb1-la1)==0)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / s;
        float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / s;

        if ((t >= 0 && t <= 1) && (u >= 0 && u <= 1))
        {
            intersection = (la2 - la1) * t+la1;

            return 1;
        }

        return 0;
    }
    public Matrix4x4 ShearMatrix(float sxy = 0, float sxz = 0, float syx = 0, float syz = 0, float szx = 0, float szy = 0)
        {
        return new Matrix4x4()
        {
            m00 = 1,
            m01 = sxy,
            m02 = sxz,
            m03 = 0,

            m10 = syx,
            m11 = 1,
            m12 = syz,
            m13 = 0,

            m20 = szx,
            m21 = szy,
            m22 = 1,
            m23 = 0,

            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1,

        };

    }

    public Vector3 Limit(Vector3 vector, float maxLength)
    {
        float length = vector.magnitude;
        if (length > maxLength)
        {
            vector = vector.normalized * maxLength;
        }
        return vector;
    }
}

//string path = AssetDatabase.GetAssetPath(Selection.activeObject);
//if (path == "")
//{
//    path = "Assets";
//}
//else if (Path.GetExtension(path) != "")
//{
//    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
//}

//public int LineAgainstPolygon(Vector2 l1, Vector2 l2, Vector2[] p)
//{
//    var result1 = ContainsPoint(p, l1);
//    var result2 = ContainsPoint(p, l2);
//    if (result1 == 0 && result2 == 0 )
//    {
//        return 0;
//    }
//    if (result1 == 1 && result2 == -1)
//        return 1;
//    return -1;
//}
//for (int i = 0; i < p1.Length; i++)
//{
//    var result = ContainsPoint(p2, p1[i] + offset1, out Vector2 l1, out Vector2 l2);
//    if (result == 1)
//    {
//        return true;
//    }
//    else if(result == 0)
//    {

//    }
//}
//for (int i = 0; i < p2.Length; i++)
//    if (ContainsPoint(p1, p2[i] + offset2)==1)
//        return true;
//return false;

//float ma = (la2.y - la1.y) / (la2.x - la1.x);
//float ca = la2.y - ma * la2.x;
//float mb = (lb2.y - lb1.y) / (lb2.x - lb1.x);
//float cb = lb2.y - mb * lb2.x;

//float signa1 = mb * la1.x + cb - la1.y;
//float signa2 = mb * la2.x + cb - la2.y;
//float signb1 = ma * lb1.x + ca - lb1.y;
//float signb2 = ma * lb2.x + ca - lb2.y;

//float diffa = signa2 - signa1;
//float diffb = signb2 - signb1;

//if (Mathf.Abs(diffa) < 0.01f || Mathf.Abs(diffb) < 0.01f)
//    return false;

//bool aSameSide = (signa1 * signa2>0);
//bool bSameSide = (signb1 * signb2>0);

//if (!aSameSide && !bSameSide)
//    return true;
/// <summary>
//	This makes it easy to create, name and place unique new ScriptableObject asset files.
/// </summary>