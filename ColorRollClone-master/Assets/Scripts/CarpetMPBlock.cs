using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetMPBlock : MonoBehaviour
{
    public MaterialPropertyBlock Block { get; private set; }

    private MeshFilter meshFilter;

    public Material[] sharedMaterials;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        Block = new MaterialPropertyBlock();
    }

    void Update()
    {
        for(int i = 0; i<sharedMaterials.Length; i++)
        {
            Graphics.DrawMesh(meshFilter.mesh, transform.position, transform.rotation, sharedMaterials[i], 0, null, 0, Block);
        }
    }

    public float GetPitch()
    {
        return sharedMaterials[0].GetFloat("_Pitch");
    }

    public float GetAnglePerUnit()
    {
        return sharedMaterials[0].GetFloat("_AnglePerUnit");
    }

}
