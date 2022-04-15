using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSController : MonoBehaviour
{
    private ParticleSystem ps;

    public Action PSPlayingDone = delegate { };

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        ps.Play();

        new DelayAction(this, () => {

            PSPlayingDone?.Invoke();

        }, (ps.main.startLifetime.constant));
    }
}
