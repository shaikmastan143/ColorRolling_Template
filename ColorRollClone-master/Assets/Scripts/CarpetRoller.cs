using Sieunguoimay;
using System;
using System.Collections;
using UnityEngine;

public class CarpetRoller : MonoBehaviour
{
    private CarpetMPBlock block;

    private CarpetRawMesh carpetRawMesh = null;

    public Carpet carpet { private set; get; }

    private float pitch;
    private float startAngle;
    private float anglePerUnit;

    private float unrolledAngle = 0;
    private float rolledAngle = 0;
    private float currentAngle = 0;

    public bool rollingOut = false;
    public bool rollingIn = false;
    public bool RolledIn = true;

    public IEnumerator RollingCoroutine;

    public event Action<bool, GameObject> RollStateChanged = delegate { };

    public event Action<GameObject> UnrollBegin = delegate { };
    public event Action<GameObject> OnUnrollEnd = delegate { };
    public event Action<Carpet> BeforeRollOut = delegate { };
    public event Action<Carpet> BeforeRollIn = delegate { };

    private TaskIdentity RollOutIdentity = null;


    private void Awake()
    {
        RolledIn = true;

        block = GetComponent<CarpetMPBlock>();

        carpet = GetComponent<Carpet>();

        pitch = block.GetPitch();

        anglePerUnit = block.GetAnglePerUnit();
    }

    public void HandleCarpetMeshRebuilt(CarpetRawMesh carpet)
    {
        carpetRawMesh = carpet;

        unrolledAngle = carpet.TopMost.y * anglePerUnit;

        rolledAngle = carpet.BottomMost.y * anglePerUnit;

        startAngle = Mathf.Abs(unrolledAngle) + Mathf.PI * 1.5f;

        block.Block.SetFloat("_StartAngle", startAngle);

        float fullRadius = pitch * (startAngle - rolledAngle);

        rolledAngle = (carpet.BottomMost.y - fullRadius * 0.5f) * anglePerUnit;

        RolledIn = true;

        SetUnrolledAngle(rolledAngle);
    }

    private void SetUnrolledAngle(float angle)
    {
        currentAngle = angle;

        block.Block.SetFloat("_UnrolledAngle", currentAngle);
    }

    public bool CheckMouseHitPoint(Vector2 relativePoint)
    {
        float radius = pitch * (startAngle - currentAngle);

        float upperBound = currentAngle / anglePerUnit + radius;

        if (relativePoint.y > upperBound)
        {
            return false;
        }

        float lowerBound = currentAngle / anglePerUnit - radius;

        float left = carpetRawMesh.LeftMost.x;

        float right = carpetRawMesh.RightMost.x;

        if (lowerBound > carpetRawMesh.LeftMost.y)
        {
            left = carpetRawMesh.BottomMost.x;
        }

        if (lowerBound > carpetRawMesh.RightMost.y)
        {
            right = carpetRawMesh.BottomMost.x;
        }


        if (left != right)
        {

            //Vector3 topLeft = new Vector3(left, 0, upperBound);
            //Vector3 bottomLeft = new Vector3(left, 0, lowerBound);
            //Vector3 topRight = new Vector3(right, 0, upperBound);
            //Vector3 bottomRight = new Vector3(right, 0, lowerBound);

            //Debug.DrawLine(topLeft, topRight);
            //Debug.DrawLine(topLeft, bottomLeft);
            //Debug.DrawLine(bottomRight, bottomLeft);
            //Debug.DrawLine(bottomRight, topRight);

            //Vector3 relPoint = new Vector3(relativePoint.x, 0, relativePoint.y);
            //Debug.DrawLine(relPoint, relPoint + new Vector3(1, 0, 0));


            if ((relativePoint.x > left && relativePoint.x < right) &&
                (relativePoint.y > lowerBound && relativePoint.y < upperBound))
            {
                return true;
            }
        }

        return Utils.Instance.ContainsPoint(carpetRawMesh.Polygon, relativePoint);
    }


    public void RollIn()
    {
        SoundController.Current.PlayCarpetRollInSound();
        SoundController.Current.Vibrate();

        if (RolledIn == false)
        {
            rollingIn = true;

            BeforeRollIn?.Invoke(carpet);

            RollingCoroutine = RollEnumerator(getTimeToRoll(0.1f, 0.5f, !RolledIn), !RolledIn);

            StartCoroutine(RollingCoroutine);
        }
    }

    public float RollOut(TaskIdentity rollOutIdentity = null)
    {
        SoundController.Current.PlayCarpetRollSound();
        SoundController.Current.Vibrate();

        float duration = 0;

        if (RolledIn == true)
        {
            rollingOut = true;

            RollOutIdentity = rollOutIdentity;

            duration = getTimeToRoll(0.1f, 0.5f, !RolledIn);

            BeforeRollOut?.Invoke(carpet);

            RollingCoroutine = RollEnumerator(duration, !RolledIn);

            StartCoroutine(RollingCoroutine);

        }
        return duration;
    }

    public void RollOutImmediate()
    {
        if (RolledIn == true)
        {
            RolledIn = false;

            SetUnrolledAngle(unrolledAngle);

            BeforeRollOut?.Invoke(carpet);

            RollStateChanged?.Invoke(RolledIn, gameObject);

        }
    }

    private IEnumerator RollEnumerator(float time, bool state)
    {
        float elapsedTime = 0;

        float startingAngle = currentAngle;

        float endingAngle = state ? rolledAngle : unrolledAngle;

        while (elapsedTime < time)
        {
            SetUnrolledAngle(Mathf.Lerp(startingAngle, endingAngle, (elapsedTime / time)));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (RolledIn != state)
        {
            RolledIn = state;

            RollStateChanged?.Invoke(RolledIn, gameObject);
        }

        if (!RolledIn)
        {

            SetUnrolledAngle(unrolledAngle);

            UnrollBegin?.Invoke(gameObject);

            RollOutIdentity?.caller.InvokeCaller(RollOutIdentity.id);
        }

        rollingIn = false;

        rollingOut = false;
    }

    private float getTimeToRoll(float a, float b, bool rollingIn)
    {
        if (rollingIn)
        {
            return 0.5f * Mathf.Lerp(a, b, Mathf.Max(0, Mathf.Abs(currentAngle - rolledAngle) / Mathf.Abs(unrolledAngle - rolledAngle)));
        }
        else
        {
            return Mathf.Lerp(a, b, Mathf.Max(0, Mathf.Abs(currentAngle - unrolledAngle) / Mathf.Abs(rolledAngle - unrolledAngle)));
        }
    }


}
//public void OnDrawGizmos()
//{
//    if (carpet != null)
//        for (int i = 0; i < carpet.Polygon.Length; i++)
//        {
//            var p1 = carpet.Polygon[i];
//            var p2 = carpet.Polygon[(i == carpet.Polygon.Length - 1) ? 0 : (i + 1)] ;
//            Debug.DrawLine(new Vector3(p1.x, 0, p1.y), new Vector3(p2.x, 0, p2.y));
//        }
//}

//public IEnumerator Roll()
//{
//    if (!Rolled)
//    {
//        if (OnBeforeRoll != null)
//            yield return StartCoroutine(OnBeforeRoll(gameObject));
//    }
//    else
//        OnBeforeUnroll?.Invoke(gameObject);

//    RollingCoroutine = RollEnumerator(getTimeToRoll(0.1f, 0.5f, !Rolled), !Rolled);
//    yield return StartCoroutine(RollingCoroutine);

//}



// Update is called once per frame
//void Update()
//{
//if (shouldRoll && !autoRolling)
//{
//    shouldRoll = false;
//    Roll();
//}

//if (isBeingUnrolled)
//{
//    if (currentAngle != unrolledAngle)
//    {
//        isBeingUnrolled = false;
//        OnUnrollEnd?.Invoke(gameObject);
//    }
//}

//if (Input.GetMouseButtonUp(0))
//{
//    OnMouseButtonUp();
//}
//if (Input.GetMouseButton(0))
//{
//    OnMouseButtonDown();
//}
//}



//public void OnMouseButtonDown()
//{
//    bool isMouseDrag = mouseButtonDownTime > MOUSE_DOWN_TIME;
//    if (mouseButtonDownTime < MOUSE_DOWN_TIME)
//        mouseButtonDownTime += Time.deltaTime;

//    if (InputManager.Current.hit)
//    {
//        ////horizontal plane only
//        //Plane plane = new Plane(transform.up, -transform.position.y);
//        //if (plane.Raycast(ray, out float enter))
//        //{
//        hitPoint = InputManager.Current.raycastHit.point;// ray.GetPoint(enter);

//        Vector3 relativeHitPoint = transform.InverseTransformPoint(hitPoint);

//        hitting = CheckMouseHitPoint(new Vector2(relativeHitPoint.x, relativeHitPoint.z));

//        if (isMouseDrag && !autoRolling)
//        {
//            if (hitting)
//            {
//                if (isFirstTouch)
//                {
//                    isFirstTouch = false;
//                    firstMouse = hitPoint;
//                    Vector3 pivot = (transform.TransformPoint(new Vector3(0, 0, carpet.BottomMost.y)));
//                    firstTouchRollingOffset = Vector3.Dot(hitPoint - pivot, transform.forward);
//                    isRollingIn = Rolled;

//                    //if (autoRolling)
//                    //{
//                    //    StopCoroutine(rollingRountine);
//                    //    autoRolling = false;
//                    //}

//                }


//                float rollingDistance = Vector3.Dot(hitPoint - firstMouse, transform.forward);
//                float unrolledLength = firstTouchRollingOffset + rollingDistance;
//                float lerpValue = unrolledLength / carpetLength;

//                //setUnrolledAngle(Mathf.Lerp(rolledAngle, unrolledAngle, lerpValue));

//                //float diff = rollingDistance - lastRollingDistance;
//                //if (Mathf.Abs(diff) > 0.05f)
//                //    isRollingIn = (diff > 0);

//                lastRollingDistance = rollingDistance;
//                rolling = (lerpValue > 0.0f && lerpValue < 1.0f);
//            }
//            else if (!isFirstTouch)
//            {
//                //isFirstTouch = true;
//                //if (rolling)
//                //{
//                //    rollingRountine = roll(getTimeToRoll(0.1f, 0.5f, !isRollingIn), !isRollingIn);
//                //    StartCoroutine(rollingRountine);
//                //}
//            }
//        }
//    }
//}
//public void OnMouseButtonUp()
//{
//    if (hitting)
//    {
//        bool isMouseClick = mouseButtonDownTime < MOUSE_DOWN_TIME;

//        //touching and release the mouse
//        isFirstTouch = true;
//        if (isMouseClick)
//        {
//            //onBeforeRoll = temp;
//            StartCoroutine(Roll());
//        }
//        else
//        {
//            if (!autoRolling && rolling)
//            {
//                RollingCoroutine = roll(getTimeToRoll(0.1f, 0.5f, !isRollingIn), !isRollingIn);
//                StartCoroutine(RollingCoroutine);
//            }
//        }
//    }
//    mouseButtonDownTime = 0;
//}


//private Vector3 hitPoint;
//private Vector3 firstMouse;
//private bool isFirstTouch = true;
//private float firstTouchRollingOffset = 0.0f;


//private float lastRollingDistance = 0.0f;
//private float mouseButtonDownTime = 0.0f;
//private const float MOUSE_DOWN_TIME = 0.2f;


//private bool isRollingIn = false;
//private float carpetLength;
//private bool hitting;

//private bool rolling = false;