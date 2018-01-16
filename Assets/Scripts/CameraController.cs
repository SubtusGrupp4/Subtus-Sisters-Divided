using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private Transform playerTop;
    [SerializeField]
    private Transform playerBot;
    [SerializeField]
    private float followSpeed = 30f;
    [SerializeField]
    private float offset;
    [SerializeField]
    private float deadZone;

    [SerializeField]
    private float midPlayers;
    [SerializeField]
    private float camDistance;

    private void FixedUpdate()
    {
        // Get the X distance from the camera to the players
        midPlayers = playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2;
        camDistance = midPlayers - transform.position.x;

        // Move camera towards the midpoint of the players
        if (camDistance > deadZone && camDistance > deadZone)
            transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
        else if (camDistance < -deadZone && camDistance < -deadZone)
            transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left
    }
}
