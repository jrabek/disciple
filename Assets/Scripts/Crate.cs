using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MovingObject
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (!collision.CompareTag("Player"))
        //{
        //    return;
        //}

        //Vector3 playerPosition = collision.transform.position;
        //Vector3 position = transform.position;

        //// TODO: Convert this to use tile map coordinates
        //float horizontalDelta = playerPosition.x - position.x;
        //int horizontal = horizontalDelta > 0 ? -1 : 1;
        //float verticalDelta = playerPosition.y - position.y;
        //int vertical = verticalDelta > 0 ? -1 : 1;
        //if (verticalDelta > horizontalDelta)
        //{
        //    horizontal = 0;
        //} else
        //{
        //    vertical = 0;
        //}

        //print("Moving crate h:" + horizontal + " v:" + vertical);

        //Move(horizontal, vertical, out RaycastHit2D hit);

        // TODO: If it can't move then we need to prevent the player from moving.
        // Better to abort the current move?  Or maybe work it into the CanMove
        // check?

        // Maybe we enable collision layer detection for actors
        // then when the player can't move we can see if the raycast hit the crate
        // so move the crate then reattempt the move with the player.
        // This will all happen on the players turn so no need to worry about
        // other characters landing on the same tile

        // Maybe crates don't move themselves?  When a player collides with it,
        // the player attempts to move it (after the raycast).  If that returns false
        // then the player aborts.
    }

    protected override void MoveComplete()
    {       
    }
}
