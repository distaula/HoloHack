﻿using HoloToolkit.Unity.InputModule;
using System.Linq;
using UnityEngine;

public class FallingObjects : MonoBehaviour, IInputClickHandler
{

    //Variables
    private GameController gameController;
    private AudioSource objectSound;

    // Use this for initialization
    void Start()
    {

        //Get GameController
        GameObject gamecontroller = GameObject.FindGameObjectWithTag("GameController");
        gameController = gamecontroller.GetComponent<GameController>();

        //Get falling sound
        objectSound = gameObject.GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        var normal = collision.contacts.Select(contact => contact.normal).Aggregate((n1, n2) => n1+n2) / collision.contacts.Count(); // Or average over multiple
        collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(collision.gameObject.GetComponent<Rigidbody>().velocity, normal);

        //Hit the ground
        objectSound.Pause();
        
        //Destroy before they fall forever
        if (collision.gameObject.tag == "Finish")
        {
            Destroy(gameObject);
        }
        
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
}