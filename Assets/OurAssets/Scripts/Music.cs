using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	public GameObject player;
	Vector3 playerPos;
	// Use this for initialization
	void Start () {
		playerPos = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = playerPos;
		playerPos = player.transform.position;
	}
}
