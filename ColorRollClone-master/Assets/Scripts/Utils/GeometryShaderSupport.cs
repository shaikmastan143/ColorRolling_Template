using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryShaderSupport : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        bool supported = SystemInfo.supportsComputeShaders;//IOS OPenglES 3.1 - 

        //SystemInfo.graphicsDeviceType;
        Debug.Log(SystemInfo.graphicsDeviceVersion);

        if (!supported)
        {
            Destroy(gameObject);
        }
    }

}
