using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cameraComponent;

    public Matrix4x4 ShearMatrix { private set; get; }

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();

        ShearMatrix = Utils.Instance.ShearMatrix(sxy: 0.125f,szy:0.25f);// syz: 0.25f);

        cameraComponent.worldToCameraMatrix *= ShearMatrix;
    }

    public void HandleGameSceneSizeChanged(Bounds bounds)
    {
        cameraComponent.orthographicSize = Mathf.Max(16,bounds.size.x + 4);
    }
}
