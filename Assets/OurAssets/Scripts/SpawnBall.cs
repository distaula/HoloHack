using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBall : MonoBehaviour, IInputClickHandler
{

        public void OnInputClicked(InputClickedEventData eventData)
        {
            var gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gameController.SpawnBall();
        }
}
