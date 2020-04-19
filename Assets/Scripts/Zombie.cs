using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameManager.instance.AddEnemy(this);
    }    

    public override void RunAI()
    {
        base.RunAI();

        Vector3Int zombiePosition = CurrentTransformToTile();
        Vector3Int playerPosition = TransformToTile(player.transform.position);        
        
        int horizontalDelta = zombiePosition.x - playerPosition.x;
        int horizontal = horizontalDelta > 0 ? -1 : 1;
        int verticalDelta = zombiePosition.y - playerPosition.y;
        int vertical = verticalDelta > 0 ? -1 : 1;

        bool couldMove = false;
        if (Mathf.Abs(verticalDelta) > Mathf.Abs(horizontalDelta))
        {            
            couldMove = base.Move(0, vertical, out _);

            if (!couldMove)
            {
                base.Move(horizontal, 0, out _);
            }
        } else
        {
            couldMove = base.Move(horizontal, 0, out _);

            if (!couldMove)
            {
                base.Move(0, vertical, out _);
            }
        }                        
    }

    protected override bool AllowedToMoveCrate()
    {
        return false;
    }

}
