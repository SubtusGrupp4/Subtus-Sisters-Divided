using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private Transform playerTop;
    [SerializeField]
    private Transform playerBot;
    [SerializeField]
    public float followSpeed = 30f;
    private float startFollowSpeed;
    [SerializeField]
    private float offset;
    [SerializeField]
    private float deadZone;

    [SerializeField]
    public float followPos;
    [SerializeField]
    private float camDistance;

    private float camWait = 0f;

    private void Start()
    {
        startFollowSpeed = followSpeed;
    }

    private void FixedUpdate()
    {


        if (GameObject.Find("DialogueManager") == null || !DialogueManager.instance.moveCamera && !DialogueManager.instance.freezeCamera)
        {
            // Get the X distance from the camera to the players
            followPos = playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2;
            camDistance = followPos - transform.position.x;

            // Move camera towards the midpoint of the players
            if (camDistance > deadZone)
                transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
            else if (camDistance < -deadZone && camDistance < -deadZone)
                transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left
        }
        else
        {
            camDistance = followPos - transform.position.x;

            transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);  // Move Right

            // Camera Move Finished
            if (camDistance < 0.1f)
            {
                StartCoroutine(CameraWait());
            }
        }
    }

    public void DialogueMove(float moveCameraSpeed, float moveCameraX, float moveCameraWait)
    {
        followSpeed = moveCameraSpeed;
        followPos = followPos + moveCameraX;
        camWait = moveCameraWait;
    }

    private IEnumerator CameraWait()
    {
        yield return new WaitForSeconds(camWait);
        if(GameObject.Find("DialogueManager") != null)
            DialogueManager.instance.moveCamera = false;
        followSpeed = startFollowSpeed;
    }
}
