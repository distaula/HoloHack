using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{

    public float leftWidth;
    public float rightWidth;
    public float height;
    public float awayDepth;

    //The ball prefabs we'll drop- set in the Unity editor
    public GameObject[] balls;

    //List of spawned fruits in play
    List<GameObject> spawnedBalls;

    private AudioSource gameOverSound;

    void Start()
    {
        //Get bomb sound
        gameOverSound = gameObject.GetComponent<AudioSource>();

        spawnedBalls = new List<GameObject>();

        //Values chosen to spawn near view frame. Feel free to play around with them.
        var cam = Camera.main;
        leftWidth = cam.transform.position.x - 1;
        rightWidth = cam.transform.position.x + 1;
        height = cam.transform.position.y + 1.5f;
        awayDepth = cam.transform.position.z + 2;

        //Start spawning fruits
        //StartCoroutine(SpawnWaves());

        AttachSpawnScripts();
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

            Respawn(spawnedBalls[spawnedBalls.Count - 1]);
            
            //Wait before spawning another
            yield return new WaitForSeconds(5f);
        }
    }

    public void SpawnBall()
    {
        //Spawn the fruit within the player's view
        Vector3 spawnPosition = new Vector3(Random.Range(leftWidth, rightWidth), height, Random.Range(awayDepth, awayDepth + 2));

        //Instatiate random fruit
        spawnedBalls.Add(Instantiate(balls[0], spawnPosition, Random.rotation));
        var vel = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1));
        spawnedBalls[spawnedBalls.Count - 1].GetComponent<Rigidbody>().velocity = vel;

        Respawn(spawnedBalls[spawnedBalls.Count - 1]);
    }
    
    /// <summary>
    /// Used to respawn the ball inside the room the player is in
    /// </summary>
    /// <param name="go"></param>
    public void Respawn(GameObject go)
    {
        StartCoroutine(_respawn(go));
    }

    public IEnumerator _respawn(GameObject go)
    {
        var tries = 0;
        while (true)
        {
            if (tries++ > 50)
                yield break;
            var camera = GameObject.FindGameObjectWithTag("MainCamera");
            var randomSpawnDir = new Vector3(Random.Range(0f, 1), Random.Range(.4f, 1), Random.Range(0f, 1));
            randomSpawnDir.Normalize();

            var pos = camera.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(pos, randomSpawnDir, out hit))
            {
                // If we hit the outside wall or the respawn loc would be too close, try again
                if (hit.collider.gameObject.tag == "Respawn" || hit.distance < 2f)
                {
                    yield return new WaitForSeconds(.25f);
                    continue;
                }
                
                var dist = hit.distance * 0.8f;
                Vector3.Scale(randomSpawnDir, new Vector3(dist, dist, dist));
                go.transform.position = pos + randomSpawnDir;
                yield break;
            }
            // Our ray hit nothing, RED ALERT, but we can probably just ignorore it
            yield break;
        }
    }

    private void AttachSpawnScripts()
    {
        var objs = GameObject.FindGameObjectsWithTag("Respawn");
        objs.Select(o => o.AddComponent<SpawnBall>());
        
        var map = GameObject.FindGameObjectWithTag("Map");
        map.AddComponent<SpawnBall>();
    }
    
    public void KillPlayer() // Pass in sound effect based on ball?
    {
        // Multiple lives? Would have different action for losing a life and dying altogether

        // Change scenes, play sound
        Debug.Log("Player Kilt");
        SceneManager.LoadScene("Empty");
    }
}