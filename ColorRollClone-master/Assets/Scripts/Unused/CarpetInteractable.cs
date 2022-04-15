using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CarpetInteractable : MonoBehaviour
{

    private Material material;
    private MeshRenderer meshRenderer;
    private float unrolledAngle = 0;
    private float angle = 0;
    private bool rolling = false;
    private bool autoRolling = false;
    private IEnumerator rollingRountine;


    private bool hit;


    public bool Rolled { get; private set; } = false;
    public Bounds DynamicBounds { get; private set; }


    private float pitch;
    private float startAngle;
    private float anglePerUnit;

    private Vector3 hitPoint;
    private Vector3 firstMouse;
    private bool isFirstTouch = true;
    private float firstTouchRollingOffset = 0.0f;
    private float lastRollingDifference = 0.0f;
    private float touchTime = 0.0f;
    private bool isRollingIn = false;
    private float rollingTime = 0.5f;
    private const float TOUCH_TIME = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Carpet interactable component");
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.materials[0];

        pitch = material.GetFloat("_Pitch");
        anglePerUnit = material.GetFloat("_AnglePerUnit");
        ResetUnrolledAngle();


        //float t = 0.5f;
        //Vector2 [] vertices2D = new Vector2[] {
        //    new Vector2(-t,-t),
        //    new Vector2(-t,t*0.5f),
        //    new Vector2(0,0),
        //    new Vector2(t,t*0.5f),
        //    new Vector2(t,-t),
        //};

        //conventionally, two first vertices are used to form the pivot edge.

        //PolygonParallelSubdivisor parallelSubdivisor = new PolygonParallelSubdivisor();
        //vertices2D = parallelSubdivisor.Subdivide(vertices2D, 0.2f, out int[] indices);

        //Triangulator triangulator = new Triangulator(vertices2D);
        //int[] indices = triangulator.Triangulate();


    }

    // Update is called once per frame
    void Update()
    {
        if (rolling||autoRolling)
        {
            float lerpValue = (unrolledAngle - angle) / unrolledAngle;
            float rolledRadius =  pitch * startAngle;
            float radius = Mathf.Lerp(meshRenderer.bounds.size.y, rolledRadius, lerpValue);
            float posY = Mathf.Lerp(meshRenderer.bounds.center.y, meshRenderer.bounds.center.y + rolledRadius, lerpValue);
            float sizeZ = Mathf.Lerp(meshRenderer.bounds.size.z, rolledRadius*2, lerpValue);
            float posZ = Mathf.Lerp(meshRenderer.bounds.center.z, transform.position.z, lerpValue);
            DynamicBounds = new Bounds()
            {
                center = new Vector3(DynamicBounds.center.x, posY, posZ),
                size = new Vector3(DynamicBounds.size.x, radius * 2, sizeZ)
            };
        }

        if (Input.GetMouseButtonUp(0))
        {
            //RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (DynamicBounds.IntersectRay(ray))
            //{
            //    if (Rolled != -1)
            //    {
            //        Rolled = (Rolled == 0 ? 1 : 0);
            //        StartCoroutine(roll(1.0f, Rolled));
            //    }
            //}
            //if (!isFirstTouch&& !rolling)
            //{
            //    isFirstTouch = true;
            //    StartCoroutine(roll(0.5f, isRollingIn));
            //}

            //if (hit)
            //{
            //    if (touchTime < TOUCH_TIME)
            //    {
            //        rollingRountine = roll(getRollingTime(!Rolled), !Rolled) ;
            //        StartCoroutine(rollingRountine);
            //    }
            //    else
            //    {
            //        if (!isFirstTouch)
            //        {
            //            if (!autoRolling && rolling)
            //            {
            //                rollingRountine = roll(getRollingTime(isRollingIn), isRollingIn);
            //                StartCoroutine(rollingRountine);
            //            }
            //            rolling = false;
            //            isFirstTouch = true;
            //            Debug.Log("End of First touch on button up");
            //        }
            //    }
            //    touchTime = 0;
            //    hit = false;
            //}
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(transform.up, -transform.position.y);
            hitPoint = new Vector3();
            if (plane.Raycast(ray, out float enter))
                hitPoint = ray.GetPoint(enter);

            hit = DynamicBounds.IntersectRay(ray);
            //isPointInsideRect(new Vector2(hitPoint.x, hitPoint.z), new Vector2(DynamicBounds.size.x, DynamicBounds.size.z), new Vector2(DynamicBounds.center.x, DynamicBounds.center.z))
            if (hit)
            {
                //Debug.Log("Mouse is touching something"+ Input.mousePosition);
                if (touchTime < TOUCH_TIME)
                {
                    touchTime += Time.deltaTime;
                    return;
                }

                if (isFirstTouch)
                {
                    isFirstTouch = false;
                    firstMouse = hitPoint;
                    firstTouchRollingOffset = Vector3.Dot(hitPoint - transform.position, transform.forward);
                    isRollingIn = !Rolled;
                    if (autoRolling)
                    {
                        StopCoroutine(rollingRountine);
                        autoRolling = false;
                    }
                }
                Vector3 diff = hitPoint - firstMouse;
                float rollingDistance = Vector3.Dot(diff, transform.forward);
                float unrolledLength = firstTouchRollingOffset + rollingDistance;
                float lerpValue = unrolledLength / meshRenderer.bounds.size.z;
                angle = Mathf.Lerp(0, unrolledAngle, lerpValue);
                material?.SetFloat("_UnrolledAngle", angle);

                if (rollingDistance - lastRollingDifference != 0)
                    isRollingIn = (rollingDistance - lastRollingDifference > 0);

                lastRollingDifference = rollingDistance;
                rolling = (lerpValue > 0.0f && lerpValue < 1.0f);
            }
            //else if(!isFirstTouch)
            //{
            //    if (!autoRolling && rolling)
            //    {
            //        rollingRountine = roll(getRollingTime(isRollingIn), isRollingIn);
            //        StartCoroutine(rollingRountine);
            //    }
            //    rolling = false;
            //    isFirstTouch = true;
            //}
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(DynamicBounds.center, DynamicBounds.size);
        Gizmos.DrawCube(hitPoint, new Vector3(0.1f, 0.1f, 0.1f));
    }
    private IEnumerator roll(float time, bool state)
    {
        Debug.Log("Rolling");
        float elapsedTime = 0;
        float startingAngle = angle;
        float endingAngle = state ? unrolledAngle : 0;
        autoRolling = true;
        while (elapsedTime < time)
        {

            angle = Mathf.Lerp(startingAngle, endingAngle, (elapsedTime / time));
            material?.SetFloat("_UnrolledAngle", angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Rolled = state;
        autoRolling = false;
    }

    public void ResetUnrolledAngle()
    {
        unrolledAngle = meshRenderer.bounds.size.z * anglePerUnit;
        material?.SetFloat("_UnrolledAngle", unrolledAngle);
        angle = unrolledAngle;
        DynamicBounds = meshRenderer.bounds;

        startAngle = Mathf.Abs(unrolledAngle)+Mathf.PI*2f;// material.GetFloat("_StartAngle");
        material.SetFloat("_StartAngle", startAngle);

    }
    private float getRollingTime(bool rollingIn)
    {
        return 0.5f;// Mathf.Max(0.01f,0.005f * (rollingIn ? Mathf.Abs(angle) : Mathf.Abs(unrolledAngle - angle)));
    }

    private bool isPointInsideRect(Vector2 point, Vector2 size, Vector2 pos)
    {
        if (point.x > pos.x + size.x) return false;
        if (point.x < pos.x - size.x) return false;
        if (point.y > pos.y + size.y) return false;
        if (point.y < pos.y - size.y) return false;
        return true;
    }
}
