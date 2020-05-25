using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingObject : CheckPointObject
{
    private float moveTime = 0.1f;

    public float MoveTime
    {
        get
        {
            return moveTime;
        }
        protected set
        {
            moveTime = value;
            inverseMoveTime = 1f / moveTime;
        }
    }

    public LayerMask blockingLayer;            //Layer on which collision will be checked.

    //[SerializeField]
    //private Vector3 transformOffset;

    private BoxCollider2D boxCollider;         //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;                //The Rigidbody2D component attached to this object.
    private float inverseMoveTime;            //Used to make movement more efficient.

    private Vector2 moveScale;
    private Tilemap floor;
    private Tilemap obstacles;
    private Tilemap walls;
    private Vector3Int currentTilePos;
    private Vector3Int targetTilePos;

    //Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start()
    {
        //Get a component reference to this object's BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();

        //Get a component reference to this object's Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();

        //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
        inverseMoveTime = 1f / moveTime;
        
        moveScale = GameManager.instance.gridSize;
 
        floor = GameManager.instance.grid.transform.Find("Floor").GetComponent<Tilemap>();
        obstacles = GameManager.instance.grid.transform.Find("Obstacles").GetComponent<Tilemap>();
        walls = GameManager.instance.grid.transform.Find("Walls").GetComponent<Tilemap>();

        Assert.IsNotNull(floor);
        Assert.IsNotNull(obstacles);

        if (BlockedByLight())
        {
            blockingLayer |= LayerMask.GetMask("Light");
        }

        currentTilePos = CurrentTransformToTile();
        // Force alignment
        transform.position = TileToTransform(currentTilePos);
    }

    public override void SaveState()
    {        
        SavePosition(transform.position);
        SaveVector3Int("currentTilePos", currentTilePos);
    }

    public override void LoadState()
    {
        transform.position = RestorePosition();
        currentTilePos = RestoreVector3Int("currentTilePos");
    }

    protected Vector3Int CurrentTransformToTile()
    {
        return TransformToTile(transform.position);
    }

    protected Vector3 TileToTransform(Vector3Int tilePosition)
    {
        return new Vector3(tilePosition.x * moveScale.x, tilePosition.y * moveScale.y, 0);
    }

    protected Vector3Int TransformToTile(Vector3 position)
    {       
        return new Vector3Int((int)(position.x / moveScale.x), (int)(position.y / moveScale.y), 0);
    }

    //Move returns true if it is able to move and false if not. 
    //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
    public bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        if (xDir == 0 && yDir == 0)
        {
            MoveComplete();
            hit = new RaycastHit2D();
            return true;
        }

        //Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;

        Vector2 castStart = start;

        const float epsilon = 0.005f;

        // Cast the ray from outside the box collider to avoid hitting our own collider
        if (xDir < 0)
        {
            castStart = new Vector2(start.x + boxCollider.offset.x - boxCollider.size.x / 2 - epsilon, start.y);
        }
        else if (xDir > 0)
        {
            castStart = new Vector2(start.x + boxCollider.offset.x + boxCollider.size.x / 2 + epsilon, start.y);
        }
        else if (yDir < 0)
        {
            castStart = new Vector2(start.x, start.y + boxCollider.offset.y - boxCollider.size.y / 2 - epsilon);
        }
        else if (yDir > 0)
        {
            castStart = new Vector2(start.x, start.y + boxCollider.offset.y + boxCollider.size.y / 2 + epsilon);
        }

        // Calculate end position based on the direction parameters passed in when calling Move.
        Vector2 end = start + new Vector2(xDir * moveScale.x, yDir * moveScale.y);

        Color[] lineColors = { Color.red, Color.green, Color.blue, Color.white, Color.yellow };
        Debug.DrawLine(castStart, end, lineColors[Random.Range(0, lineColors.Length - 1)], 45f);

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(castStart, end, blockingLayer);

        if (hit.transform != null)
        {
            print(this.gameObject + " would collide with " + hit.transform);

            if (!TryOverlapTransform(hit.transform))
            {
                return false;
            }
        }
                        
        targetTilePos = new Vector3Int(currentTilePos.x + xDir, currentTilePos.y + yDir, 0);

        print(this.tag + " checking tile at " + targetTilePos);

        //Check if anything was hit
        if ((floor.HasTile(targetTilePos) || !RequiresFloorTileToMove()) && !obstacles.HasTile(targetTilePos) && !walls.HasTile(targetTilePos))
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));

            //Return true to say that Move was successful
            return true;
        } else
        {
            print(this.tag + ": Obstacle, wall, or no floor tile at " + targetTilePos);
        }

        //If something was hit, return false, Move was unsuccesful.
        return false;
    }

    private bool TryOverlapTransform(Transform transform)
    {
        if (transform.gameObject.CompareTag("Crate") && AllowedToMoveCrate())
        {
            return MoveCrate(transform.GetComponent<Crate>());
        }
        else if (transform.gameObject.CompareTag("Enemy") && this.GetComponent<Player>())
        {
            return true;
        }
        else if (transform.gameObject.CompareTag("Player") && KillsPlayer())
        {
            return true;
        }
        else if (transform.CompareTag("Light") && !BlockedByLight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual bool RequiresFloorTileToMove()
    {
        return true;
    }

    protected virtual bool KillsPlayer()
    {
        return false;
    }

    protected virtual bool AllowedToMoveCrate()
    {
        return true;
    }

    public virtual bool BlockedByLight()
    {
        return false;
    }

    private bool MoveCrate(Crate crate)
    {
        Vector3 playerPosition = transform.position;
        Vector3 cratePosition = crate.transform.position;

        // TODO: Convert this to use tile map coordinates
        float horizontalDelta = playerPosition.x - cratePosition.x;
        int horizontal = horizontalDelta > 0 ? -1 : 1;
        float verticalDelta = playerPosition.y - cratePosition.y;
        int vertical = verticalDelta > 0 ? -1 : 1;
        if (Mathf.Abs(verticalDelta) > Mathf.Abs(horizontalDelta))
        {
            horizontal = 0;
        }
        else
        {
            vertical = 0;
        }

        // print("Attempting to move crate h:" + horizontal + "("+horizontalDelta+") v:" + vertical + " ("+verticalDelta+")");

        return crate.Move(horizontal, vertical, out _);
    }

    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // print(this.tag + " moving smoothly to " + TransformToTile(end));

        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude becae it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        float epsilon = moveScale.x / 100;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > epsilon)
        {            
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        transform.position = TileToTransform(targetTilePos);
        currentTilePos = targetTilePos;

        // print(this.tag + " finishing moving to " + TransformToTile(transform.position));

        MoveComplete();
    }


    ////The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
    ////AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
    protected virtual bool AttemptMove(int xDir, int yDir, out GameObject hitObject)
    {
        // print(this + " attemptMove xDir " + xDir + " yDir " + yDir);

        //Hit will store whatever our linecast hits when Move is called.
        RaycastHit2D hit;

        //Set canMove to true if Move was successful, false if failed.
        bool canMove = Move(xDir, yDir, out hit);

        if (hit) {
            hitObject = hit.transform.gameObject;
        } else
        {
            hitObject = null;
        }
   
        return canMove;
    }

    protected abstract void MoveComplete();
}