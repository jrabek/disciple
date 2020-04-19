using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;        //Allows us to use SceneManager
using UnityEngine.Assertions;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{       
    private Animator animator;                    //Used to store a reference to the Player's animator component.
    
    private GameManager gameManager;

    [SerializeField]
    private int time = 100;

    public int souls { get; private set; } = 0;
    private const int baseSoulCapacity = 10;
    public const int maxSoulCapacity = baseSoulCapacity * 2;
    public int soulCapacity { get; private set; } = baseSoulCapacity;

    private const int dashLength = 2;

    // Powers
    private bool dashEnabled = false;
    private bool pushCratesEnabled = false;
    private bool killDemonEnabled = false;

    private int pendingSouls = 0;

    private int facing = 1;
    private bool pushCratesWasEnabled = false;

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        
        //Call the Start function of the MovingObject base class.
        base.Start();

        gameManager = GameManager.instance;

        Assert.IsNotNull(gameManager);

        gameManager.UpdateSouls(souls);
        gameManager.UpdateTime(time);
    }


    private void Update()
    {
        //If it's not the player's turn, exit the function.
        if (!gameManager.playersTurn) return;

        int horizontal = 0;      //Used to store the horizontal move direction.
        int vertical = 0;        //Used to store the vertical move direction.

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : 0);

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0)
        {
            facing = horizontal;
            transform.localScale = new Vector3(facing, transform.localScale.y, 1);
        }


        GameObject hitObject;

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {            
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove(horizontal, vertical, out hitObject);

            if (hitObject)
            {
                print("Ran into " + hitObject);
            }
        } else if (Input.GetKeyDown(KeyCode.Space) && dashEnabled)
        {
            pushCratesWasEnabled = pushCratesEnabled;
            pushCratesEnabled = false;
            AttemptMove(facing * dashLength, 0, out hitObject);

            if (hitObject)
            {
                print("Can't dash due to " + hitObject);
            }
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override bool AttemptMove(int xDir, int yDir, out GameObject hitObject)
    {        
        gameManager.PlayerStartAction();

        //Every time player moves, subtract from food points total.
        time -= (Mathf.Abs(xDir) + Mathf.Abs(yDir));

        gameManager.UpdateTime(time);

        //If Move returns true, meaning Player was able to move into an empty space.
        if (base.AttemptMove(xDir, yDir, out hitObject))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            animator.SetTrigger("Run");

            //Since the player has moved and lost food points, check if the game has ended.
            CheckIfGameOver();
            return true;
        } else {
            // print("AttemptMove Failed");
            MoveComplete();
            return false;
        }        
    }

    protected override bool AllowedToMoveCrate()
    {
        return pushCratesEnabled;
    }

    protected override void MoveComplete()
    {
        pushCratesEnabled = pushCratesWasEnabled;
        animator.SetTrigger("Idle");
        gameManager.PlayerFinishAction();
    }


    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        ////Check if the tag of the trigger collided with is Exit.
        //if (other.tag == "Exit")
        //{
        //    //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
        //    Invoke("Restart", restartLevelDelay);

        //    //Disable the player object since level is over.
        //    enabled = false;
        //}

        ////Check if the tag of the trigger collided with is Food.
        //else if (other.tag == "Food")
        //{
        //    //Add pointsPerFood to the players current food total.
        //    spirit += pointsPerFood;

        //    //Disable the food object the player collided with.
        //    other.gameObject.SetActive(false);
        //}

        ////Check if the tag of the trigger collided with is Soda.
        //else if (other.tag == "Soda")
        //{
        //    //Add pointsPerSoda to players food points total
        //    spirit += pointsPerSoda;


        //    //Disable the soda object the player collided with.
        //    other.gameObject.SetActive(false);
        //}
    }


    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene(0);
    }

    
    //It takes a parameter loss which specifies how many points to lose.
    public void LoseTime(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        time -= loss;

        //Check to see if game has ended.
        CheckIfGameOver();
    }


    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (time <= 0)
        {

            //Call the GameOver function of GameManager.
            gameManager.GameOver();
        }
    }

    public bool CouldAddSoul(int count = 1)
    {
        if (souls + count + pendingSouls < soulCapacity)
        {
            pendingSouls += count;
            return true;
        } else
        {
            return false;
        }
    }    

    public void CantAddSouls()
    {
        gameManager.IndicateFullSouls();
    }

    public void AddSoul(int count = 1)
    {
        souls += count;
        pendingSouls -= count;
        gameManager.UpdateSouls(souls);
    }

    public void RemoveSoul(int count = 1)
    {
        souls -= count;
        souls = souls < 0 ? 0 : souls;        
        gameManager.UpdateSouls(souls);
    }

    public void EnableDoubleSouls()
    {
        soulCapacity = baseSoulCapacity * 2;
    }

    public void EnableDash()
    {
        dashEnabled = true;
    }

    public void EnablePushCrates()
    {
        pushCratesWasEnabled = pushCratesEnabled = true;
    }

    public void EnableKillDemon()
    {
        killDemonEnabled = true;
    }
}