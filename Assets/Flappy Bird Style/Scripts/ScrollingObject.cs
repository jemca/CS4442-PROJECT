using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingObject : MonoBehaviour 
{
	public GameControl gameControl;

	private Rigidbody2D rb2d;
	

	// Use this for initialization
	void Start () 
	{
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();

		//Start the object moving.

		//NEW
		rb2d.velocity = new Vector2 (gameControl.scrollSpeed, 0);

		
		//OLD
		// rb2d.velocity = new Vector2 (GameControl.instance.scrollSpeed, 0);
	}

	void Update()
	{
		// If the game is over, stop scrolling.
		
		//NEW
		if(gameControl.gameOver == true)

		//OLD
		// if(GameControl.instance.gameOver == true)
		{
			rb2d.velocity = Vector2.zero;
		}

		else
		{
			rb2d.velocity = new Vector2 (gameControl.scrollSpeed, 0);

		}
		
		
	}
}
