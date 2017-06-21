using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public float leftWidth;
    public float rightWidth;
    public float height;
    public float awayDepth;

    //The fruit prefabs we'll drop- set in the Unity editor
    public GameObject[] fruits;

    //Score/Timer
    private int score;
    private float timer;

    //List of spawned fruits in play
    List<GameObject> spawnedFruits;

    private AudioSource bombSound;

    void Start()
    {
        //Set score and timer to start values
        score = 0;
        timer = 120f;

        //Get bomb sound
        bombSound = gameObject.GetComponent<AudioSource>();

        spawnedFruits = new List<GameObject>();

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
            spawnedFruits.Add(Instantiate(fruits[Random.Range(0, 6)], spawnPosition, Random.rotation));

            //Wait before spawning another
            yield return new WaitForSeconds(3f);
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

        while (spawnedFruits.Count > 0)
        {
            Destroy(spawnedFruits[0]);
            spawnedFruits.RemoveAt(0);
        }

        //Take away time and score
        timer -= 30;
        score -= 500;
    }
}