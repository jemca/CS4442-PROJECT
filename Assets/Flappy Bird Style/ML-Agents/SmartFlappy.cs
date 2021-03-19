﻿using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SmartFlappy : Agent
{
    public float upForce; //Upward force of the "flap".
    private bool isDead = false; //Has the player collided with a wall?

    private Animator anim; //Reference to the Animator component.
    private Rigidbody2D rb2d; //Holds a reference to the Rigidbody2D component of the bird.

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(-2, 2, 0);
        GameControl.instance.RestartGame();
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
        Debug.Log(Time.time + " " + actions.DiscreteActions[0]);

        if (isDead == false)
        {
            AddReward(+10);
            if (actions.DiscreteActions[0] == 1)
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
        GameControl.instance.BirdDied();

        SetReward(-1000);
        EndEpisode();
    }
}