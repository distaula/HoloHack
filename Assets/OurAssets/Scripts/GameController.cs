using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public float leftWidth;
    public float rightWidth;
    public float height;
    public float awayDepth;

    //The ball prefabs we'll drop- set in the Unity editor
    public GameObject[] balls;

    //Score/Timer
    private int score;
    private float timer;

    //List of spawned fruits in play
    List<GameObject> spawnedBalls;

    private AudioSource bombSound;

    void Start()
    {
        //Set score and timer to start values
        score = 0;
        timer = 120f;

        //Get bomb sound
        bombSound = gameObject.GetComponent<AudioSource>();

        spawnedBalls = new List<GameObject>();

        //Values chosen to spawn near view frame. Feel free to play around with them.
        var cam = Camera.main;
        leftWidth = cam.transform.position.x - 1;
        rightWidth = cam.transform.position.x + 1;
        height = cam.transform.position.y + 1.5f;
        awayDepth = cam.transform.position.z + 2;

        //Start spawning fruits
        StartCoroutine(SpawnWaves());
    }

    //Spawn fruits
    IEnumerator SpawnWaves()
    {
        var cam = Camera.main;
        while (true)
        {
            //Spawn the fruit within the player's view
            Vector3 spawnPosition = new Vector3(Random.Range(leftWidth, rightWidth), height, Random.Range(awayDepth, awayDepth + 2));

            //Instatiate random fruit
            spawnedBalls.Add(Instantiate(balls[0], spawnPosition, Random.rotation));
            spawnedBalls[spawnedBalls.Count - 1].GetComponent<Rigidbody>().velocity = new Vector3(0, -1f, 0);

            //Wait before spawning another
            yield return new WaitForSeconds(.5f);
        }
    }
    //Adds Score when tapping fruit
    public void AddScore()
    {
        score += 100;
    }

    //This function is called every fixed framerate frame
    void FixedUpdate()
    {
        //Update Timer and Screen Text
        timer -= Time.deltaTime;
    }

    //Called by bomb to clear field 
    public void BombClear()
    {
        bombSound.Play();

        while (spawnedBalls.Count > 0)
        {
            Destroy(spawnedBalls[0]);
            spawnedBalls.RemoveAt(0);
        }

        //Take away time and score
        timer -= 30;
        score -= 500;
    }

    // Balls2Walls
    public void KillPlayer() // Pass in sound effect based on ball?
    {
        // Multiple lives? Would have different action for losing a life and dying altogether

        // Change scenes, play sound
    }
}