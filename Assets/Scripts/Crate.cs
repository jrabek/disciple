using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Assertions;

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

        Assert.IsNotNull(candleLight);
    }

    public override void SaveState()
    {
        base.SaveState();
        SaveInt("litPushesLeft", litPushesLeft);        
    }

    public override void LoadState()
    {
        base.LoadState();
        litPushesLeft = RestoreInt("litPushesLeft");
        candleLight.enabled = litPushesLeft > 0;
    }

    protected override void MoveComplete()
    {
        if (litPushesLeft <= 0)
        {
            return;
        }

        if (--litPushesLeft == 0 && candleAnimator != null && candleAnimator.enabled)
        {
            candleAnimator.SetTrigger("Extinguish");
            candleLight.enabled = false;
        }
    }

    protected override bool AllowedToMoveCrate()
    {
        return false;
    }

}
