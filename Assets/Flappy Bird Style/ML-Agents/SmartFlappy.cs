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

    public bool heuristicOnly;

    public Sprite initialSprite;
    public override void OnEpisodeBegin()
    {
        
        columnPool.ResetColumns();
        
        anim.SetTrigger("Reset");
        rb2d.velocity = Vector2.zero;
        // rb2d.angularVelocity = Single.NaN;
        transform.localPosition = new Vector3(-2, Random.Range(-1.5f,4.5f), 0);
        // transform.localRotation = Quaternion.Euler(0, 0, 0); //Quaternion.identity);
        // gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;

        isDead = false;

        // Debug.Log("episode begin");
        
        // transform.localPosition = new Vector3(-2, 2, 0);

        passedCol = 0;
        
        //NEW
        gameControl.RestartGame();
        //OLD
        // GameControl.instance.RestartGame();
    }


    private void Start()
    {
        
        
        //Get reference to the Animator component attached to this GameObject.
        anim = GetComponent<Animator>();
        //Get and store a reference to the Rigidbody2D attached to this GameObject.
        rb2d = GetComponent<Rigidbody2D>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log(Time.time + " " + actions.DiscreteActions[0]);

        if (isDead == false)
        {
            AddReward(+1);
            
            if (actions.DiscreteActions[0] == 0)
            {
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

        SetReward(-1000);
        EndEpisode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SCORE"))
        {
            //If the bird hits the trigger collider in between the columns then
            //tell the game control that the bird scored.
            
            //NEW
            gameControl.BirdScored();
            //OLD
            // GameControl.instance.BirdScored();
            
            AddReward(+100);
            passedCol++;
            Debug.Log(Time.time + "PASSED COLUMN:" + passedCol);
        }
    }
}