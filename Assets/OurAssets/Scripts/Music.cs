using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    GameObject player;
	Vector3 playerPos;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
		playerPos = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = playerPos;
		playerPos = player.transform.position;
	}
}
