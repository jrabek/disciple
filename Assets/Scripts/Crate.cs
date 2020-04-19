using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Crate : MovingObject
{
    [SerializeField]
    int litPushesLeft = 3;

    private Animator candleAnimator;
    private Light2D candleLight;

    public bool candleIsLit { get; private set; } = true;

    protected override void Start()
    {
        base.Start();
        candleAnimator = GetComponentInChildren<Animator>();
        candleLight = GetComponentInChildren<Light2D>();
    }

    protected override void MoveComplete()
    {
        if (--litPushesLeft == 0)
        {
            candleAnimator.SetTrigger("Extinguish");
            candleLight.enabled = false;
        }
    }
}
