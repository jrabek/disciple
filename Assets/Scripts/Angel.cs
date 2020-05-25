using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : Enemy
{
    private Animator animator;
    private HashSet<Transform> candles = new HashSet<Transform>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();                
    }

    protected override int MovesPerTurn()
    {
        return 2;
    }

    public override void RunAI()
    {
        base.RunAI();

        // print($"Running AI for {this.gameObject}");

        // Don't do anything if in a light
        if (candles.Count > 0)
        {
            // print("Angel Fleeing: collding with " + candles.Count + " candles");

            // TODO: Add a fleeing/damage animation
            animator.SetTrigger("Idle");

            Transform[] transformsToAvoid = new Transform[candles.Count];
            candles.CopyTo(transformsToAvoid);

            FleeAI(transformsToAvoid);

            return;
        }

        if (WithinChaseDistance())
        {
            animator.SetTrigger("Activate");
            // print("Angel within chase distance");
        } else
        {
            animator.SetTrigger("Idle");
            // print("Angel outside chase distance");
        }

        // print("Angel Chasing");
        ChaseAI();
    }

    protected override void MoveComplete()
    {
        base.MoveComplete();

        if (movesLeft == 0 && candles.Count > 0)
        {
            // Angel dies if it can't escape the candle
            animator.SetTrigger("Die");
        }
    }

    public void DeathAnimationComplete()
    {
        this.gameObject.SetActive(false);
    }

    public override bool VulnerableToLight()
    {
        return true;
    }

    public override bool BlockedByLight()
    {
        // If the Angel is in candle light it should
        // flee        
        return candles.Count == 0;
    }

    protected override bool RequiresFloorTileToMove()
    {
        return false; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // TODO: Later can add a child object to player with this tag to get
        // this behavior for free
        if (collision.CompareTag("Light"))
        {
            candles.Add(collision.transform);
        }
        // print("Angel trigger entered " + collision.gameObject + " candles.Count:" + candles.Count);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // TODO: Later can add a child object to player with this tag to get
        // this behavior for free        
        if (collision.CompareTag("Light"))
        {
            candles.Remove(collision.transform);
        }
        // print("Angel trigger exited " + collision.gameObject + " candles.Count:" + candles.Count);
    }
}
