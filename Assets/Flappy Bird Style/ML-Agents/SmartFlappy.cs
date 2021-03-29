using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
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
    public int life = 0;

    public bool heuristicOnly;

    [Tooltip("Whether this is training mode or gameplay mode")]
    public bool trainingMode;

    public GameObject currentTarget;
    public int currentTargetIndex = 0;


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
        if (!trainingMode) MaxStep = 0;
    }


    public override void OnEpisodeBegin()
    {
        // columnPool.ResetColumns();

        anim.SetTrigger("Reset");
        rb2d.velocity = Vector2.zero;
        // rb2d.angularVelocity = Single.NaN;
        transform.localPosition = new Vector3(-2, Random.Range(-1.5f, 4.5f), 0);
        // transform.localRotation = Quaternion.Euler(0, 0, 0); //Quaternion.identity);
        // gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;

        isDead = false;

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
        currentTarget = columnPool.targets[currentTargetIndex];
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
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(currentTarget.transform.localPosition);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log(Time.time + " AI input: " + actions.DiscreteActions[0]);

        if (trainingMode && isDead == false)
        {
            //same thing
            // Debug.Log(this.StepCount);
            // Debug.Log(life);

            // life++;
            // var bonus = 0.01f * life;
            // AddReward(bonus);


            if (actions.DiscreteActions[0] == 0)
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
        heuristicOnly = true;
    }

    private void Update()
    {
        //Don't allow control if the bird has died.
        if (isDead == false && heuristicOnly)
        {
            //Look for input to trigger a "flap".
            if (Input.GetMouseButtonDown(0))
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

        if (other.gameObject.CompareTag("BOUNDARY"))
        {
            AddReward(-1000);
            // Debug.Log("hit boundary");
        }
        else if (other.gameObject.CompareTag("COLUMN"))
        {
            AddReward(-1000);
            // Debug.Log("hit column");
        }


        // Zero out the bird's velocity
        rb2d.velocity = Vector2.zero;
        // If the bird collides with something set it to dead...
        isDead = true;
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
            
            var targetReward = 1000 + 200 * passedCol;
            passedCol++;

            AddReward(targetReward);

            if (passedCol > 5) Debug.Log(Time.time + "PASSED COLUMN:" + passedCol + " " + targetReward);

            currentTargetIndex = (currentTargetIndex + 1) % columnPool.columnPoolSize;

            // Debug.Log("current target index:" + currentTargetIndex);

            currentTarget = columnPool.targets[currentTargetIndex];
        }
    }
}