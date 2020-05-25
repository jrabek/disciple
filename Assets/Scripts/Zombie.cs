using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MoveTime = 0.5f;
    }    

    public override void RunAI()
    {
        base.RunAI();
        ChaseAI();
    }
}
