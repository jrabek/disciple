using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MovingObject
{
    protected Player player;

    protected override void Start()
    {
        //Call the Start function of the MovingObject base class.
        base.Start();

        player = GameObject.FindObjectOfType<Player>();
    }

    protected override void MoveComplete()
    {        
    }

    protected void UpdateFacingDirection()
    {
        float horizontal = transform.position.x - player.transform.position.x > 0 ? -1 : 1;
        transform.localScale = new Vector3(horizontal, transform.localScale.y, 1);
    }

    protected override bool KillsPlayer()
    {
        return true;
    }

    public virtual void RunAI()
    {
        UpdateFacingDirection();
    }
}
