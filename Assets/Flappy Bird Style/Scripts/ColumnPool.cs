using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ColumnPool : MonoBehaviour
{
    // public SmartFlappy bird;
    // public GameObject columnPrefab; //The column game object.
    // public GameObject targetPrefab; //SCORE POINTS

    public GameControl gameControl;

    public float spawnRate = 3f; //How quickly columns spawn.
    public float columnMin = -1f; //Minimum y value of the column position.
    public float columnMax = 3.5f; //Maximum y value of the column position.

    public int currentColumn = 0; //Index of the current column in the collection.

    public Vector2 objectPoolPosition = new Vector2(-15, -25); //A holding position for our unused columns offscreen.

    
    
    
    
    public float spawnXPosition = 10f;

    private float timeSinceLastSpawned;

    [Space(20)] public int columnPoolSize = 4; //How many columns to keep on standby.
    public GameObject[] columns; //Collection of pooled columns.
    public GameObject[] targets; //Collection of pooled columns.


    public float[] initialPositions;  

    void Start()
    {
        timeSinceLastSpawned = 0f;

        // initialPositions = new float[columnPoolSize];
        // for (int i = 0; i < columnPoolSize; i++)
        // {
        //     initialPositions[i] = columns[i].transform.position.x;
        // }


        // // CREATED IN THE SCENE NOT NEEDED ANYMORE
        // //Initialize the columns collection.
        // columns = new GameObject[columnPoolSize];
        // targets = new GameObject[columnPoolSize];
        // //Loop through the collection... 
        // for (int i = 0; i < columnPoolSize; i++)
        // {
        //     //...and create the individual columns.
        //     columns[i] = Instantiate(columnPrefab, objectPoolPosition, Quaternion.identity, transform.parent);
        //     columns[i].GetComponent<ScrollingObject>().gameControl = gameControl;
        //     
        //     targets[i] = Instantiate(targetPrefab, objectPoolPosition, Quaternion.identity, transform.parent);
        //     targets[i].GetComponent<ScrollingObject>().gameControl = gameControl;
        //     
        //     
        //     
        // }
    }


    //This spawns columns as long as the game is not over.
    // void Update()
    // {
    //     timeSinceLastSpawned += Time.deltaTime;
    //
    //     //NEW
    //     if (gameControl.gameOver == false && timeSinceLastSpawned >= spawnRate)
    //
    //         //OLD	
    //         // if (GameControl.instance.gameOver == false && timeSinceLastSpawned >= spawnRate) 
    //     {
    //         timeSinceLastSpawned = 0f;
    //
    //         //Set a random y position for the column
    //         float spawnYPosition = Random.Range(columnMin, columnMax);
    //
    //         //...then set the current column to that position.
    //         columns[currentColumn].transform.localPosition = new Vector3(spawnXPosition, spawnYPosition, 0);
    //         targets[currentColumn].transform.localPosition = new Vector3(spawnXPosition, spawnYPosition, 0);
    //
    //
    //         //Increase the value of currentColumn. If the new size is too big, set it back to zero
    //         currentColumn++;
    //
    //         if (currentColumn >= columnPoolSize)
    //         {
    //             currentColumn = 0;
    //         }
    //
    //
    //
    //         
    //     }
    //
    //
    // }


    private void Update()
    {
        if (gameControl.gameOver == false && columns[currentColumn].transform.position.x < -10)
        {
            float spawnYPosition = Random.Range(columnMin, columnMax);

            //...then set the current column to that position.
            columns[currentColumn].transform.localPosition = new Vector3(spawnXPosition, spawnYPosition, 0);
            targets[currentColumn].transform.localPosition = new Vector3(spawnXPosition, spawnYPosition, 0);


            currentColumn = (currentColumn + 1) % columnPoolSize;
        }
    }

    public void ResetColumns()
    {
        
        for (int i = 0; i < columnPoolSize; i++)
        {
            float spawnYPosition = Random.Range(columnMin, columnMax);

            columns[i].transform.localPosition = new Vector3(initialPositions[i], spawnYPosition, 0);
            targets[i].transform.localPosition = new Vector3(initialPositions[i], spawnYPosition, 0);
    
            currentColumn = 0;
        }
        
        
        
        
        
        
        
    //  old one   
    // for (int i = 0; i < columnPoolSize; i++)
    // {
    //     columns[i].transform.localPosition = objectPoolPosition;
    //     targets[i].transform.localPosition = objectPoolPosition;
    //
    //     currentColumn = 0;
    // }
    }
    
    
}