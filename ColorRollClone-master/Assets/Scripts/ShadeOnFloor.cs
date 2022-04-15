using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShadeOnFloor : SamplePainter
{
    protected MaterialPropertyBlock block;
    
    private void Awake()
    {
        block = new MaterialPropertyBlock();

        block.SetColor("_Color", DefaultColor);
    }

    public override void Paint(LevelSO levelSO, bool useSameColor = false)
    {
        currentLevelSO = levelSO;
        if (currentLevelSO == null)
        {
            return;
        }
        GenerateMesh(currentLevelSO.carpetSOs, useSameColor);

        var x = currentLevelSO.Position.x * canvas.transform.localScale.x;
        var z = currentLevelSO.Position.y * canvas.transform.localScale.z;

        canvas.transform.localPosition = new Vector3(x, 0.01f, z);
    }

    private void Update()
    {
        if (polygons == null)
        {
            return;
        }

        for (int i = polygons.Length - 1; i >= 0; i--)
        {
            Graphics.DrawMesh(polygons[i], canvas.transform.localToWorldMatrix, material, 0, null, 0, block);
        }

    }
}