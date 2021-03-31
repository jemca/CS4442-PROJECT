using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SmartFlappy : Agent
{
    public GameControl gameControl; // this was a singleton before
    public ColumnPool columnPool;

    public float upForce; //Upward force of the "flap".
    private bool isDead = false; //Has the player collided with a wall?

    private Animator anim; //Reference to the Animator component.
    private Rigidbody2D rb2d; //Holds a reference to the Rigidbody2D component of the bird.

    public int passedCol = 0;
    public int maxColumn=0;
    public int life = 0;

    public bool heuristicOnly;

    [Tooltip("Whether this is training mode or gameplay mode")]
    // public bool trainingMode;
    public GameObject currentTarget;

    public GameObject currentEnemy;
    public GameObject currentTunnel;

    public int currentTargetIndex = 0;

    public float targetBoxHeightFromCenter;

    public float birdColumnGap;
    public float tunnelBonus;


    // private Vector3 birdPosition;
    // private Vector3 tunnelPosition;
    // private Vector3 targetPosition;


    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        //Get reference to the Animator component attached to this GameObject.
        anim = GetComponent<Animator>();
        //Get and store a reference to the Rigidbody2D attached to this GameObject.
        rb2d = GetComponent<Rigidbody2D>();


        // If not training mode, no max step, play forever
        // if (!trainingMode) MaxStep = 0;
    }


    public override void OnEpisodeBegin()
    {
        isDead = false;

        // columnPool.ResetColumns();

        anim.SetTrigger("Reset");
        rb2d.velocity = Vector2.zero;
        // rb2d.angularVelocity = Single.NaN;
        transform.localPosition = new Vector3(-2, Random.Range(-1.5f, 4.5f), 0);
        // transform.localRotation = Quaternion.Euler(0, 0, 0); //Quaternion.identity);
        // gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;


        // Debug.Log("episode begin");

        // transform.localPosition = new Vector3(-2, 2, 0);

        passedCol = 0;
        life = 0;


        //NEW
        gameControl.RestartGame();
        columnPool.ResetColumns();


        //OLD
        // GameControl.instance.RestartGame();


        currentTargetIndex = 0;
        currentEnemy = columnPool.columns[currentTargetIndex];
        currentTarget = columnPool.targets[currentTargetIndex];
        currentTunnel = columnPool.tunnels[currentTargetIndex];
    }


    // OLD
    // private void Start()
    // {
    //     //Get reference to the Animator component attached to this GameObject.
    //     anim = GetComponent<Animator>();
    //     //Get and store a reference to the Rigidbody2D attached to this GameObject.
    //     rb2d = GetComponent<Rigidbody2D>();
    // }

    public override void CollectObservations(VectorSensor sensor)
    {
        var birdPosition = transform.localPosition;
        var tunnelPosition = currentTunnel.transform.localPosition;
        var targetPosition = currentTarget.transform.localPosition;

        var velocity = rb2d.velocity.y;
        var enemyPosition = currentEnemy.transform.localPosition.x;


        sensor.AddObservation(birdPosition);
        sensor.AddObservation(tunnelPosition);

        // Get a vector from the beak tip to the nearest flower
        // Vector3 toTargetDirection = targetPosition - birdPosition;

        // sensor.AddObservation(toTargetDirection.normalized);
        sensor.AddObservation(targetPosition);

        sensor.AddObservation(velocity);
        sensor.AddObservation(enemyPosition);
        
        
        
        

        // Debug.Log(enemyPosition);
        // Debug.Log($"bird:{birdPosition} target{targetPosition} totarget{toTargetDirection} normalized{toTargetDirection.normalized} velocityY{rb2d.velocity.y}" );
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // IT TAKES VALUES EVEN IF NOT TRAINING
        // Debug.Log(Time.time + " AI input: " + actions.DiscreteActions[0]);

        if (isDead == false)
        {
            //same thing
            // Debug.Log(this.StepCount);
            // Debug.Log(life);


            //OLD STAY ALIVE BONUS ACTUALLY WORKED
            // life++;
            // var bonus = 0.01f * life;
            // AddReward(bonus);

            //NEW STAY ALIVE AND STAY ALIGNED BONUS
            var birdX = transform.localPosition.x;
            var enemyX = currentEnemy.transform.localPosition.x;
            var gap = enemyX - birdX;

            if (gap > birdColumnGap)
            {
                var birdY = transform.localPosition.y;
                var targetY = currentTarget.transform.localPosition.y;
                var top = targetY + targetBoxHeightFromCenter;
                var bottom = targetY - targetBoxHeightFromCenter;

                if (birdY < top && birdY > bottom)
                {
                    // Debug.Log($"birdy{birdY} top{top} bottom{bottom}  ");
                    var bonus = 1f; //* StepCount;
                    AddReward(bonus);
                    life = StepCount;
                }
            }


            if (actions.DiscreteActions[0] == 1)
            {
                // Debug.Log("agent received input to fly");

                //...tell the animator about it and then...
                anim.SetTrigger("Flap");
                //...zero out the birds current y velocity before...
                rb2d.velocity = Vector2.zero;
                //	new Vector2(rb2d.velocity.x, 0);
                //..giving the bird some upward force.
                rb2d.AddForce(new Vector2(0, upForce));
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        // discreteActions[0] = Input.GetMouseButtonDown(0) ? 0 : 1;


        heuristicOnly = true;
    }

    private void Update()
    {
        //Don't allow control if the bird has died.
        if (heuristicOnly && isDead == false)
        {
            //Look for input to trigger a "flap".
            if (Input.GetMouseButtonDown(0) == true)
            {
                // Debug.Log("key pressed to fly");

                //...tell the animator about it and then...
                anim.SetTrigger("Flap");
                //...zero out the birds current y velocity before...
                rb2d.velocity = Vector2.zero;
                //	new Vector2(rb2d.velocity.x, 0);
                //..giving the bird some upward force.
                rb2d.AddForce(new Vector2(0, upForce));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Debug.Log(other.gameObject.tag);

        // If you wanna separate penalties but looks redundant
        // if (other.gameObject.CompareTag("BOUNDARY"))
        // {
        //     AddReward(-1000);
        //     // Debug.Log("hit boundary");
        // }
        // else if (other.gameObject.CompareTag("COLUMN"))
        // {
        //     AddReward(-1000);
        //     // Debug.Log("hit column");
        // }

        // AddReward(-1000);
        AddReward(-1);


        // Zero out the bird's velocity
        rb2d.velocity = Vector2.zero;
        // If the bird collides with something set it to dead...
        isDead = true;

        // tunnelBonus = 0;
        
        //...tell the Animator about it...
        anim.SetTrigger("Die");
        //...and tell the game control about it.

        //NEW
        gameControl.BirdDied();
        //OLD
        // GameControl.instance.BirdDied();


        EndEpisode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SCORE"))
        {
            // Time.timeScale = 0;
    
            //If the bird hits the trigger collider in between the columns then
            //tell the game control that the bird scored.
    
            //NEW
            gameControl.BirdScored();
            //OLD
            // GameControl.instance.BirdScored();
    
            // var targetReward = 1000 + 200 * passedCol;

            var targetReward = 1;

            passedCol++;
    
            AddReward(targetReward);

            // if (passedCol > maxColumn)
            // {
            //     maxColumn = passedCol;
            //     if(maxColumn > 1000) Debug.Log(Time.time + "PASSED COLUMN:" + passedCol + " " + targetReward);
            //
            // }
                
                
                
    
            currentTargetIndex = (currentTargetIndex + 1) % columnPool.columnPoolSize;
    
            // Debug.Log("current target index:" + currentTargetIndex);
    
            currentEnemy = columnPool.columns[currentTargetIndex];
            currentTarget = columnPool.targets[currentTargetIndex];
            currentTunnel = columnPool.tunnels[currentTargetIndex];
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TUNNEL"))
        {
            tunnelBonus += 0.1f; //* StepCount;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TUNNEL"))
        {
            
            AddReward(tunnelBonus);
            // Debug.Log(tunnelBonus);

            tunnelBonus = 0;
        }

    }
}