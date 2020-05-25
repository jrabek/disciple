using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MovingObject
{
    [SerializeField]
    protected int maxManhattanChaseDistance = 10;

    protected bool chasing = false;

    protected Player player;

    protected int movesLeft;

    protected override void Start()
    {        
        base.Start();
        MoveTime = 0.3f;
        player = GameObject.FindObjectOfType<Player>();
        GameManager.instance.AddEnemy(this);
    }

    protected override void MoveComplete()
    {
        movesLeft--;
        if (movesLeft > 0)
        {
            RunAI();
        } else
        {
            GameManager.instance.EnemyFinishAction();
        }
    }

    protected override bool AllowedToMoveCrate()
    {
        return false;
    }

    public virtual bool VulnerableToLight()
    {
        return false;
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


    protected virtual int MovesPerTurn()
    {
        return 1;
    }

    public virtual void StartAI()
    {
        movesLeft = MovesPerTurn();
        RunAI();
    }

    public virtual void RunAI()
    {
        UpdateFacingDirection();        
    }

    public virtual bool AIComplete()
    {
        return movesLeft == 0;
    }

    protected Vector2Int PlayerDistance()
    {
        return DistanceToTransform(player.transform);        
    }

    protected Vector2Int DistanceToTransform(Transform otherTransform)
    {
        Vector3Int otherTransformPosition = TransformToTile(otherTransform.position);
        Vector3Int position = CurrentTransformToTile();
        int horizontalDelta = position.x - otherTransformPosition.x;
        int verticalDelta = position.y - otherTransformPosition.y;

        return new Vector2Int(horizontalDelta, verticalDelta);
    }

    protected bool WithinChaseDistance()
    {
        Vector2 playerDistance = PlayerDistance();
        return Mathf.Abs(playerDistance.x) + Mathf.Abs(playerDistance.y) <= maxManhattanChaseDistance;
    }  

    protected virtual void ChaseAI()
    {
        Vector2 playerDistance = PlayerDistance();
        int xDir = playerDistance.x > 0 ? -1 : playerDistance.x < 0 ? 1 : 0;
        int yDir = playerDistance.y > 0 ? -1 : playerDistance.y < 0 ? 1 : 0;

        if (!WithinChaseDistance())
        {
            MoveComplete();
            return;
        }

        if (yDir == 0 && xDir == 0)
        {
            MoveComplete();
            return;
        }

        // print($"ChaseAI: {this.gameObject} going x: {xDir} y: {yDir}");

        bool preferVertical = Mathf.Abs(playerDistance.y) > Mathf.Abs(playerDistance.x);
        if (!AttemptMoves(xDir, yDir, preferVertical))
        {
             MoveComplete();
        }
    }

    protected virtual void FleeAI(Transform[] transformsToAvoid)
    {
        float horizontalSum = 0;
        float verticalSum = 0;

        foreach (Transform avoidTransform in transformsToAvoid)
        {
            Vector2 transformDistance = DistanceToTransform(avoidTransform);
            
            horizontalSum += 1.0f / (float)transformDistance.x;
            verticalSum += 1.0f / (float)transformDistance.y;

           // print($"transformDistance:{transformDistance} horizontalSum:{horizontalSum} verticalSum:{verticalSum}");
        }
        
        int xDir = horizontalSum < 0 ? -1 : horizontalSum > 0 ? 1 : 0;
        int yDir = verticalSum < 0 ? -1 : verticalSum > 0 ? 1 : 0;

        // print($"FleeAI: {this.gameObject} fleeing x: {xDir} ({horizontalSum}) y: {yDir} ({verticalSum})");

        bool preferVertical = Mathf.Abs(verticalSum) > Mathf.Abs(horizontalSum);
        if (!AttemptMoves(xDir, yDir, preferVertical))
        {
            MoveComplete();
        }
    }

    protected virtual bool AttemptMoves(int xDir, int yDir, bool preferVertical)
    {
        bool couldMove = false;
        if (preferVertical)
        {
            couldMove = base.AttemptMove(0, yDir, out _) || base.AttemptMove(xDir, 0, out _);
        }
        else
        {
            couldMove = base.AttemptMove(xDir, 0, out _) || base.AttemptMove(0, yDir, out _);
        }

        return couldMove;
    }
}
