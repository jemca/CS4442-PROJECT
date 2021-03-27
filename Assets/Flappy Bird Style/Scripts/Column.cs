using UnityEngine;
using System.Collections;

public class Column : MonoBehaviour
{

	public bool columnScoreEnabled = false;
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(columnScoreEnabled && other.GetComponent<Bird>() != null)
		{
			//If the bird hits the trigger collider in between the columns then
			//tell the game control that the bird scored.
			GameControl.instance.BirdScored();
			
		}
	}
}
