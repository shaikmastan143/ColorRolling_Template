using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Action<int, Vector3> MouseButtonDown = delegate { };

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown?.Invoke(0, Input.mousePosition);
        }
    }
}





////int layerMask = LayerMask.GetMask("Ground", "Environment");
//hit = Physics.Raycast(ray, out raycastHit, Mathf.Infinity/*, layerMask*/);
//if (hit)
//{
//    Debug.Log("ProcessMouseClick: " + raycastHit.transform.gameObject.name);

//    OnClick?.Invoke(raycastHit);
//}


