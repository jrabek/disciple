using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Assertions;

public class Crate : MovingObject
{
    [SerializeField]
    int litPushesLeft = 4;

    private Candle candle;
    
    protected override void Start()
    {
        base.Start();
        candle = GetComponentInChildren<Candle>();
        Assert.IsNotNull(candle);
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
        candle.Light(litPushesLeft > 0);        
    }

    protected override void MoveComplete()
    {
        if (litPushesLeft <= 0)
        {
            return;
        }

        if (--litPushesLeft == 0)
        {
            candle.Light(false);
        }
    }

    protected override bool AllowedToMoveCrate()
    {
        return false;
    }
}
