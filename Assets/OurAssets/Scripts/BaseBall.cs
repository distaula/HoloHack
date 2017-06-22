﻿using HoloToolkit.Unity.InputModule;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class BaseBall : MonoBehaviour
{
    //Variables
    private GameController gameController;
    private AudioSource[] objectSound;
	public AudioSource impactSource;
	public AudioClip[] impacts; //array of different impact sfx for ball collisions]


    // Use this for initialization
    void Start()
    {
		

        //Get GameController
        GameObject gamecontroller = GameObject.FindGameObjectWithTag("GameController");
        gameController = gamecontroller.GetComponent<GameController>();

        //Get AudioSources
		objectSound = gameObject.GetComponents<AudioSource>();
		objectSound[0].pitch = Random.Range (1.1f, 0.9f);




    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Respawn")
        {
            gameController.Respawn(gameObject);
        }

        var center = gameObject.GetComponent<SphereCollider>().center + gameObject.transform.position;
        var impactPoint = collider.ClosestPointOnBounds(center);
        var dir = impactPoint - center;
        dir.Normalize();
        RaycastHit hit;
        if (collider.Raycast(new Ray(center, dir), out hit, 10))
        {
            Collide(hit);
        }
		//play random impact sound with random pitch
		impactSource.pitch = Random.Range (1.2f, 0.8f);
		impactSource.clip = impacts [Random.Range (0, 3)];
		impactSource.Play();

		

    }

    protected void Collide(RaycastHit hit)
    {
        var normal = GetCollisionNormal(hit);

        // Now reflect the object's velocity around that normal to have it bounce off
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(gameObject.GetComponent<Rigidbody>().velocity, normal);
    }

    void OnCollisionEnter(Collision collision)
    {
        //var normal = collision.contacts
        //    .Select(contact => contact.normal)
        //    .Aggregate((n1, n2) => n1 + n2) // Sum the normals
        //    / collision.contacts.Count(); // Get the average
        
        //// Now reflect the object's velocity around that normal to have it bounce off
        //gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(gameObject.GetComponent<Rigidbody>().velocity, normal);

        //// Stop the falling sound
        //objectSound.Pause();
        
        //if (collision.gameObject.tag == "Player")
        //{
        //    // I hit a player, better kill him
        //    gameController.KillPlayer();
        //    // Then destroy the ball so it doesn't instantly kill him more
        //    Destroy(gameObject);
        //}
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //Check to see if bomb- deletes all fruits
        if (gameObject.tag.Equals("Bomb"))
        {
            //Play bomb sound
            gameController.BombClear();
        }
        else
        {
            //Add to Score
            gameController.AddScore();

            Destroy(gameObject);
        }
    }

    private Vector3 GetCollisionNormal(RaycastHit hit)
    {
        if (hit.collider is SphereCollider)
        {
            //Debug.Log("Hit SphereCollider");
            return GetSphereNormal(hit);
        }

        //Debug.Log(hit.collider.gameObject.name);
        MeshCollider collider = (MeshCollider)hit.collider;
        Mesh mesh = collider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal.Normalize();
        interpolatedNormal = hit.transform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }

    private Vector3 GetSphereNormal(RaycastHit hit)
    {
        var center = ((SphereCollider)hit.collider).center + hit.collider.gameObject.transform.position;
        var dir = hit.point - center;
        dir.Normalize();
        return dir;
    }
}
