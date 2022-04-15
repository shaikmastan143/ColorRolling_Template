using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintParticle : MonoBehaviour
{
    public GameObject target;

    [SerializeField] float randomValue = 0.3f;

    private Boid boid;

    public Action Disappear = delegate { };

    void Start()
    {
        boid = GetComponent<Boid>();

        boid.Acc = new Vector3(UnityEngine.Random.Range(-randomValue, randomValue), UnityEngine.Random.Range(-randomValue, randomValue),0);

        boid.sensingRadius = 0.2f;
    }

    void Update()
    {
        if (boid.Arrive(target.transform.position))
        {
            Disappear?.Invoke();

            Destroy(gameObject);
        }
        else
        {
            boid.Step();
        }
    }

}
