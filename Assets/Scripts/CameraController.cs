using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fix dialogue moving
// TODO: Fix dialogue freezing
// TODO: Add freezing of camera
// TODO: Add players position being clamped to the view

public enum CameraState
{
    FollowingBoth, FollowingOne, Frozen, Moving
}

public class CameraController : MonoBehaviour
{
    public CameraState State;

	private Transform playerTop;
    private Transform playerBot;

    [Header("Following")]
    [SerializeField]
    public float followSpeed = 30f;     // Higher values = Slower speed
    private float startFollowSpeed;
    [SerializeField]
    private float deadZone;             // How far the players can move from the center of the screen before it starts following

    public Vector2 followPos;
    private float camDistance;

    private float camWait = 0f;

    private Transform target;

    [Header("Zooming")]
    [SerializeField]
    private bool useZooming = true;
    [SerializeField]
    private float yZoomOffset;
    [SerializeField]
    private float minZoom = 7f;
    [SerializeField]
    private float maxZoom = 11f;
    [SerializeField]
    private float zoomTime = 1f;
    private float currentZoomTime;
    private bool zoomIn = false;
    private float startZoom;
    private float startYPos;

    private void Start()
    {
        // Get the players transforms from the GameManager
        playerTop = GameManager.instance.playerTop;
        playerBot = GameManager.instance.playerBot;

        startFollowSpeed = followSpeed;
        State = CameraState.FollowingBoth;

        currentZoomTime = zoomTime;

        maxZoom = GetComponent<Camera>().orthographicSize;

        // Prevent the zooms from being 0, which crashes Unity
        if (minZoom <= 1f)
            minZoom = 7f;

        if (maxZoom <= 1f)
            maxZoom = 11f;
    }

    private void Update()
    {
        // Camera zooming depending on state
        if (useZooming)
        {
            if (State == CameraState.FollowingBoth)
            {
                if (zoomIn)
                {
                    currentZoomTime = 0f;
                    zoomIn = false;
                    startZoom = GetComponent<Camera>().orthographicSize;
                    startYPos = transform.position.y;
                }

                // Timer
                currentZoomTime += Time.deltaTime;
                if (currentZoomTime > zoomTime)
                    currentZoomTime = zoomTime;

                // Smothly transition in a S curve
                float t = currentZoomTime / zoomTime;
                t = t * t * (3f - 2f * t);

                // Lerp towards maxZoom
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoom, maxZoom, t);

                // Lerp towards the set Y position
                float yZoomMove = Mathf.Lerp(startYPos, 0f, t);
                transform.position = new Vector3(transform.position.x, yZoomMove, -10f);
            }
            else if (State == CameraState.FollowingOne)
            {
                if (!zoomIn)
                {
                    currentZoomTime = 0f;
                    zoomIn = true;
                    startZoom = GetComponent<Camera>().orthographicSize;
                    startYPos = transform.position.y;
                }

                // Timer
                currentZoomTime += Time.deltaTime;
                if (currentZoomTime > zoomTime)
                    currentZoomTime = zoomTime;

                // Smothly transition in a S curve
                float t = currentZoomTime / zoomTime;
                t = t * t * (3f - 2f * t);

                // Lerp towards minZoom
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoom, minZoom, t);

                // Choose what Y position based on what player to target
                if (target == playerBot)
                    yZoomOffset = -Mathf.Abs(yZoomOffset);
                else
                    yZoomOffset = Mathf.Abs(yZoomOffset);

                // Lerp towards the set Y position
                float yZoomMove = Mathf.Lerp(startYPos, yZoomOffset, t);
                transform.position = new Vector3(transform.position.x, yZoomMove, -10f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (State == CameraState.FollowingBoth && playerTop.gameObject.activeSelf && playerBot.gameObject.activeSelf)
        {
            // Get the X distance from the camera to the players
            followPos = new Vector2(playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2, 0f);
            camDistance = followPos.x - transform.position.x;

            // Move camera towards the midpoint of the players
            if (camDistance > deadZone)
                transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
            else if (camDistance < -deadZone && camDistance < -deadZone)
                transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left
        }
        else if (State == CameraState.FollowingOne)
        {
            // Get the X distnace from the camera to the target player
            camDistance = target.position.x - transform.position.x;

            // If the target moves even slightly from the camera X, translate towards the player
            if (camDistance > 0.1f)
                transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);      // Move Right
            else
                transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);      // Move Left

        }
        else if(State == CameraState.Moving)    // TODO: Fix this, accessed by DialogueManager
        {
            camDistance = followPos.x - transform.position.x;

            transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);  // Move Right

            // Camera Move Finished
            if (camDistance < 0.1f)
                StartCoroutine(CameraWait());
        }
    }

    public void SetCameraState(CameraState State)
    {
        this.State = State;
    }

    public void SetCameraState(CameraState State, Transform playerTarget)
    {
        this.State = State;
        if (playerTarget == playerTop)
            target = playerBot;
        else
            target = playerTop;
    }

    public void DialogueMove(float moveCameraSpeed, float moveCameraX, float moveCameraWait)
    {
        followSpeed = moveCameraSpeed;
        followPos = new Vector2(followPos.x + moveCameraX, 0f);
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
