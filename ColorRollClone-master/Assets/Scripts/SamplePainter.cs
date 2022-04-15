using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SamplePainter : MonoBehaviour
{
    [SerializeField] protected LevelSO levelSO;
    [SerializeField] protected Material material;
    [SerializeField] protected GameObject canvas;

    protected Mesh[] polygons;
    protected LevelSO currentLevelSO;
    protected MaterialPropertyBlock[] blocks;

    public Color DefaultColor = new Color(0,0,0,0);

    private const float baseWidth = 8.0f;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = canvas.transform.localScale;
    }

    public virtual void Paint(LevelSO levelSO, bool useSameColor = false)
    {
        currentLevelSO = levelSO;

        if (currentLevelSO == null)
        {
            return;
        }
        

        var bounds = GenerateMesh(currentLevelSO.carpetSOs,useSameColor);

        float ratio = baseWidth / Mathf.Max(bounds.size.x, bounds.size.y);

        canvas.transform.localScale = originalScale*ratio;

        //var x = currentLevelSO.Position.x * canvas.transform.localScale.x;
        //var z = currentLevelSO.Position.y * canvas.transform.localScale.z;
        var x = bounds.center.x * canvas.transform.localScale.x;
        var z = bounds.center.y * canvas.transform.localScale.z;
        canvas.transform.localPosition = new Vector3(-x, 0.01f, -z);
    }

    protected Bounds GenerateMesh(List<CarpetSO> carpetSOs,bool useSameColor )
    {

        int n = carpetSOs.Count;

        polygons = new Mesh[n];

        blocks = new MaterialPropertyBlock[n];

        float left = float.MaxValue, right = float.MinValue;
        float bottom = float.MaxValue, top = float.MinValue;

        for (int i = 0; i < n; i++)
        {
            var c = carpetSOs[i];

            int m = c.Polygon.Count;

            Triangulator triangulator = new Triangulator(c.Polygon.ToArray());

            int[] polygonIndices = triangulator.Triangulate();

            var vertices = new Vector3[m];

            for (int j = 0; j < m; j++)
            {
                var p = c.Position + c.Polygon[j];

                vertices[j] = new Vector3(p.x, 0, p.y);

                if (p.x < left)
                    left = p.x;
                if (p.x > right)
                    right = p.x;


                if (p.y < bottom)
                    bottom = p.y;
                if (p.y > top)
                    top = p.y;
            }

            polygons[i] = new Mesh();
            polygons[i].vertices = vertices;
            polygons[i].triangles = polygonIndices;
            polygons[i].RecalculateNormals();

            blocks[i] = new MaterialPropertyBlock();

            if (useSameColor)
            {
                blocks[i].SetColor("_Color", DefaultColor);
            }
            else
            {
                blocks[i].SetColor("_Color", c.Color);
            }

            blocks[i].SetFloat("_YOffset", (float)c.Order * 0.0001f);
        }
        return new Bounds() { size = new Vector3(right - left, top - bottom,0),center = new Vector3(right+left,top+bottom,0)*0.5f };
    }

    private void Update()
    {
        if (polygons == null)
        {
            return;
        }

        for (int i = 0; i < polygons.Length; i++)
        {
            Graphics.DrawMesh(polygons[i], canvas.transform.localToWorldMatrix, material, 0, null, 0, blocks[i]);
        }
    }
}
