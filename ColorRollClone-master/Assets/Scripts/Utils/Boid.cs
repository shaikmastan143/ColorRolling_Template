using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    public Vector3 Forward 
    {
        set { transform.forward = value; }
        get { return transform.forward; }
    }

    public Vector3 Pos { get { return transform.position; } set { transform.position = value; } }
    public Vector3 Vel;
    public Vector3 Acc;

    public Vector3 target;
    private bool reachedTarget=false;

    public float maxSpeed = 0.01f;
    public float maxForce = 0.01f;


    public float sensingRadius = 0.5f;
    private float sightSeeingRadius = 3.0f;

    public Vector3 boundPos;
    public Vector3 boundSize;

    public bool shouldFaceMovingDirection = false;
    public bool shouldCheckBound = false;

    
    
    //how it is
    public void SetUp(Vector3 pos, Vector3 vel, Vector3 acc, float maxSpeed = 0.01f, float maxForce = 0.01f)
    {
        Pos = pos;
        Vel = vel;
        Acc = acc;
        this.maxForce = maxForce;
        this.maxSpeed = maxSpeed;
    }
    
    //what it can aware of
    public void SetPercepts(Vector3 boundPos, Vector3 boundSize)
    {
        this.boundPos = boundPos;
        this.boundSize = boundSize;

    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.DrawWireCube(boundPos,boundSize);
    //    //Gizmos.DrawCube(target,new Vector3(0.5f,0.5f,0.5f));
    //    //Gizmos.DrawWireSphere(Pos, sensingRadius);
    //    Gizmos.DrawLine(Pos, Pos + Vel.normalized * 5f);
    //}

    public void Step()
    {
        Vel += Acc;
        //Vel = Utils.Instance.Limit(Vel, maxSpeed);
        Pos += Vel;
        Acc = new Vector3(0.0f, 0.0f, 0.0f);

        if (shouldFaceMovingDirection)
        {
            if (Vel.sqrMagnitude > 0.001f)
                Forward = Vel.normalized;
        }

        if(shouldCheckBound)
            ApplyForce(CheckBound2()*0.5f);
    }
    public void ApplyForce(Vector3 force)
    {
        Acc += force;
    }

    public Vector3 Seek(Vector3 target)
    {
        return Steer(target - Pos);
    }

    public Vector3 Steer(Vector3 desired)
    {
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steer = desired - Vel;
        steer = Utils.Instance.Limit(steer, maxForce);
        //ApplyForce(steer);
        return steer;
    }

    public bool Arrive(Vector3 target)
    {
        bool reached = false;
        this.target = target;

        Vector3 desired = target - Pos;
        float d = desired.magnitude;
        desired.Normalize();

        if (d < sensingRadius)
        {
            float m = Mathf.Lerp(0, maxSpeed, d/ 0.5f);
            desired *= m;
            reached = true;
        }
        else
        {
            desired *= maxSpeed;
        }
        Vector3 steer = desired - Vel;
        steer = Utils.Instance.Limit(steer, maxForce);
        ApplyForce(steer);
        return reached;
    }

    public void Flock(List<Boid>boids)
    {
        Vector3 coh = Cohesion(boids) * 0.05f;
        Vector3 sep = Separate(boids) * 0.5f;
        Vector3 ali = Align(boids) * 0.05f;
        ApplyForce(sep);
        ApplyForce(coh);
        ApplyForce(ali);
        //Debug.Log(coh + " " + ali + " " + sep);
    }

    public void CheckBound()
    {
        Vector3 newPos = new Vector3(Pos.x, Pos.y, Pos.z);
        if (Pos.x > boundPos.x + boundSize.x)
            newPos.x = boundPos.x - boundSize.x;
        else if (Pos.x < boundPos.x - boundSize.x)
            newPos.x = boundPos.x + boundSize.x;

        if (Pos.y > boundPos.y + boundSize.y)
            newPos.y = boundPos.y - boundSize.y;
        else if (Pos.y < boundPos.y - boundSize.y)
            newPos.y = boundPos.y + boundSize.y;

        if (Pos.z > boundPos.z + boundSize.z)
            newPos.z = boundPos.z - boundSize.z;
        else if (Pos.z < boundPos.z - boundSize.z)
            newPos.z = boundPos.z + boundSize.z;
        Pos = newPos;
    }

    public Vector3 CheckBound2()
    {
        Vector3 desired = Vel;
        if (Pos.x > boundPos.x + boundSize.x)
        {
            desired = new Vector3(-maxSpeed, Vel.y,Vel.z);
        }
        else if (Pos.x < boundPos.x - boundSize.x)
        {
            desired = new Vector3(maxSpeed, Vel.y, Vel.z);
        }

        if (Pos.y > boundPos.y + boundSize.y)
        {
            desired = new Vector3(Vel.x, -maxSpeed, Vel.z);
        }
        else if (Pos.y < boundPos.y - boundSize.y)
        {
            desired = new Vector3(Vel.x, maxSpeed, Vel.z);
        }

        if (Pos.z > boundPos.z + boundSize.z)
        {
            desired = new Vector3(Vel.x, Vel.y, -maxSpeed);
        }
        else if (Pos.z < boundPos.z - boundSize.z)
        {
            desired = new Vector3(Vel.x, Vel.y, maxSpeed);
        }
        Vector3 steer = Utils.Instance.Limit(desired - Vel,maxForce);
        return steer;
    }

    public Vector3 Align(List<Boid> boids)
    {

        Vector3 sum = new Vector3(0, 0, 0);
        int count = 0;
        foreach(Boid other in boids)
        {
            float d = (other.Pos - Pos).magnitude;
            if(d>0&&d< sightSeeingRadius)
            {
                sum += other.Vel;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum = sum.normalized * maxSpeed;
            Vector3 steer = sum - Vel;
            steer=Utils.Instance.Limit(steer, maxForce);
            return steer;
        }
        return new Vector3(0,0,0);
    }

    public Vector3 Cohesion(List<Boid> boids)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        int count = 0;
        foreach (Boid other in boids)
        {
            float d = (other.Pos - Pos).magnitude;
            if (d > 0 && d < sightSeeingRadius)
            {
                sum += other.Pos;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            return Seek(sum);
        }
        return new Vector3(0, 0, 0);
    }
    public Vector3 Separate(List<Boid> boids)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        int count = 0;
        foreach (Boid other in boids)
        {
            Vector3 diff = Pos-other.Pos;
            float d = (diff).magnitude;
            if (d > 0 && d < sensingRadius)
            {
                sum += (diff.normalized/d);
                count++;
            }
        }
        if (count > 0)
        {
            sum /= (float)count;
            sum = sum.normalized * maxSpeed;
            sum = Utils.Instance.Limit(sum, maxSpeed);
            Vector3 steer = sum - Vel;
            steer= Utils.Instance.Limit(steer, maxForce);
            return steer;
        }
        return new Vector3(0, 0, 0);
    }

    //public Vector3 FlyAroundObstacle(Obstacle obstacle)
    //{
    //    Vector3 diff = Pos - obstacle.transform.position;

    //    Bounds bounds = obstacle.GetBounds();
    //    if (Utils.Instance.IsInsideBounds(bounds, Pos))
    //    {

    //    }
    //}
    public Vector3 Avoid(Bounds bounds)
    {
        Vector3 diff = bounds.center - Pos;
        float radius = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.1f;
        bounds = gameObject.GetComponent<MeshRenderer>().bounds;
        radius += Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.1f;

        if (diff.magnitude < radius)
        {

            Vector3 up = Vector3.up;
            Vector3 side = Vector3.Cross(up, diff);
            Vector3 perpendicular = Vector3.Cross(side, diff);
            float sign = Vector3.Dot(perpendicular, Vel);
            perpendicular *= sign;
            return Steer(perpendicular.normalized * maxSpeed);
        }
        return new Vector3();
    }
    private float wavingTime = 0;
    private float wavingSign = 1;
    public Vector3 Waving()
    {
        if (wavingTime > 1.5f)
        {
            wavingSign = -wavingSign;
            wavingTime = 0;
        }
        wavingTime += Time.deltaTime;
        Vector3 desired = wavingSign * transform.up.normalized*maxSpeed-Vel;
        return Steer(desired);
    }
}
