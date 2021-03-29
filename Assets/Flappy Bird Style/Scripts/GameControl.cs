using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public GameObject sky1, sky2;


    // public static GameControl instance;
    // //A reference to our game control script so we can access it statically.
    public Text scoreText; //A reference to the UI text component that displays the player's score.

    public GameObject gameOvertext; //A reference to the object that displays the text which appears when the player dies.

    private int score = 0; //The player's score.
    public bool gameOver = false; //Is the game over?
    public float scrollSpeed = -1.5f;


    // void Awake()
    // {
    // 	//If we don't currently have a game control...
    // 	if (instance == null)
    // 		//...set this one to be it...
    // 		instance = this;
    // 	//...otherwise...
    // 	else if(instance != this)
    // 		//...destroy this one because it is a duplicate.
    // 		Destroy (gameObject);
    // }

    void Update()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        //If the game is over and the player has pressed some input...
        if (gameOver) // && Input.GetMouseButtonDown(0))
        {
            // Debug.Log("restarted game");

            gameOver = false;
            score = 0;

            //...reload the current scene.
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void BirdScored()
    {
        sky1.SetActive(true);
        sky2.SetActive(true);

        //The bird can't score if the game is over.
        if (gameOver)
            return;
        //If the game is not over, increase the score...
        score++;
        //...and adjust the score text.

        // TURNING OFF FOR MULTIPLE SETUPS
        // scoreText.text = "Score: " + score.ToString();
    }

    public void BirdDied()
    {
        //Activate the game over text.

        // TURNING OFF FOR MULTIPLE SETUPS
        // gameOvertext.SetActive (true);


        sky1.SetActive(false);
        sky2.SetActive(false);


        //Set the game to be over.
        gameOver = true;
    }
}