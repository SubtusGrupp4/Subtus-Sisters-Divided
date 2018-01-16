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
        /*
        // Get the X distance from the camera to the players
        float midPlayers = playerTop.position.x - playerBot.position.x;

        float distanceTop = playerTop.position.x - transform.position.x + offset;
        float distanceBot = playerBot.position.x - transform.position.x + offset;

        if (distanceTop > deadZone && distanceBot > deadZone)
            transform.Translate(new Vector2((distanceTop - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
        else if (distanceTop < -deadZone && distanceBot < -deadZone)
            transform.Translate(new Vector2((distanceTop + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left

        */

        // Get the X distance from the camera to the players
        midPlayers = playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2;
        camDistance = midPlayers - transform.position.x;
        //transform.position = new Vector3(midPlayers, 0f, -10f);

        if (camDistance > deadZone && camDistance > deadZone)
            transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
        else if (camDistance < -deadZone && camDistance < -deadZone)
            transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left
    }
}
