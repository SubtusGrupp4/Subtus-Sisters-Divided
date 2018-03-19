using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fix dialogue moving
// TODO: Fix dialogue freezing
// TODO: Add freezing of camera
// TODO: Add players position being clamped to the view

public enum CameraState
{
    FollowingBoth, FollowingOne, Transitioning, Frozen, Moving
}

public class CameraController : MonoBehaviour
{
    public CameraState State;

	private Transform playerTop;
    private Transform playerBot;

    [Header("Following")]
    public float followSpeed = 30f;     // Higher values = Slower speed
    [SerializeField]
    private float maxSpeed = 0.25f;
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
    public float maxZoom = 11f;
    [SerializeField]
    private float zoomTime = 1f;
    private float currentZoomTime;
    private bool zoomIn = false;
    private float startZoom;
    private float startYPos;
    private bool doZoom = false;
    private float to;

    [Header("Boundaries")]
    [SerializeField]
    private float minX = -10000f;
    [SerializeField]
    private float maxX = 10000f;

    private Camera cam;
    private CameraClamp clamp;

    private void Start()
    {
        // Get the players transforms from the GameManager
        playerTop = GameManager.instance.playerTop;
        playerBot = GameManager.instance.playerBot;

        startFollowSpeed = followSpeed;
        State = CameraState.FollowingBoth;

        currentZoomTime = zoomTime;

        cam = GetComponent<Camera>();
        maxZoom = cam.orthographicSize;
        startZoom = maxZoom;

        // Prevent the zooms from being 0, which crashes Unity
        GameManager.instance.PreventZero(minZoom, 1f, 7f);
        GameManager.instance.PreventZero(maxZoom, 1f, 11f);

        // Place camera on the players
        transform.position = new Vector3(playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2, 0f, -10f);

        clamp = GetComponent<CameraClamp>();
    }

    private void Update()
    {
        if (useZooming && doZoom)
            DoZoom();

        KeepInBoundry();
    }

    private void DoZoom()
    {
        // Timer
        currentZoomTime += Time.deltaTime;
        if (currentZoomTime > zoomTime)
            currentZoomTime = zoomTime;

        // Smothly transition in a S curve
        float t = currentZoomTime / zoomTime;
        t = t * t * (3f - 2f * t);

        // Lerp towards minZoom
        cam.orthographicSize = Mathf.Lerp(startZoom, to, t);

        // Choose what Y position based on what player to target
        if (GameManager.instance.onePlayerDead)
        {
            if (target == playerBot)
                yZoomOffset = -Mathf.Abs(yZoomOffset);
            else
                yZoomOffset = Mathf.Abs(yZoomOffset);

            // Lerp towards the set Y position
            float yZoomMove = Mathf.Lerp(startYPos, yZoomOffset, t);
            transform.position = new Vector3(transform.position.x, yZoomMove, -10f);
        }
        else
        {
            // Lerp towards the set Y position
            float yZoomMove = Mathf.Lerp(startYPos, yZoomOffset, t);
            transform.position = new Vector3(transform.position.x, 0f, -10f);
        }
    }

    private void Zoom()
    {
        if (State == CameraState.FollowingOne)
            to = minZoom;
        else
            to = maxZoom;

        startZoom = cam.orthographicSize;
        currentZoomTime = 0f;
        startYPos = transform.position.y;
        doZoom = true;
    }

    private void Zoom(float to)
    {
        this.to = to;

        startZoom = cam.orthographicSize;
        currentZoomTime = 0f;
        startYPos = transform.position.y;
        doZoom = true;
    }

    private void KeepInBoundry()
    {
        float boundryMargin = ((cam.orthographicSize / 9f) * 16f);

        if (transform.position.x > maxX - boundryMargin)
            transform.position = new Vector3(maxX - boundryMargin, transform.position.y, -10f);
        if (transform.position.x - boundryMargin < minX)
            transform.position = new Vector3(minX + boundryMargin, transform.position.y, -10f);
    }

    private void FixedUpdate()
    {
        if (State == CameraState.FollowingBoth && playerTop.gameObject.activeSelf && playerBot.gameObject.activeSelf)
            FollowBothMove();
        else if (State == CameraState.FollowingOne)
            FollowOneMove();
        else if (State == CameraState.Transitioning)
            Transition();
    }

    private void FollowBothMove()
    {
        // Get the X distance from the camera to the players
        followPos = new Vector2(playerTop.position.x + (playerBot.position.x - playerTop.position.x) / 2, 0f);
        camDistance = followPos.x - transform.position.x;

        // Move camera towards the midpoint of the players
        if (camDistance > deadZone)
            transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
        else if (camDistance < -deadZone)
            transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left

        if (camDistance > 0.1f || camDistance < -0.1f)
            clamp.SetClamp(true);
    }

    private void FollowOneMove()
    {
        // Get the X distnace from the camera to the target player
        camDistance = target.position.x - transform.position.x;

        // If the target moves even slightly from the camera X, translate towards the player
        if (camDistance > 0.1f || camDistance < -0.1f)
            transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);      // Move Right

        clamp.SetClamp(false);
    }

    private void Transition()
    {
        // Move towards the top safepoint
        camDistance = SafepointManager.instance.currentTopSafepoint.position.x - transform.position.x;

        // If the camera is close, enable clamping. Otherwise, disable it.
        if (camDistance < 3f && camDistance > -3f)
        {
            Debug.Log("Reached safepoint. Spawning players");
            clamp.SetClamp(true);
            SafepointManager.instance.SpawnPlayers();
            SetCameraState(CameraState.FollowingBoth);
        }
        else
            clamp.SetClamp(false);

        // Move towards safepoint with max speed maxSpeed
        float moveSpeed = camDistance / followSpeed;
        moveSpeed = Mathf.Clamp(moveSpeed, -maxSpeed, maxSpeed);

        transform.Translate(new Vector2(moveSpeed, 0.0f), Space.Self);
        
    }

    public void SetCameraState(CameraState State)
    {
        this.State = State;
        Zoom();
    }

    public void SetCameraState(CameraState State, Transform playerTarget)
    {
        this.State = State;
        if (playerTarget == playerTop)
            target = playerBot;
        else
            target = playerTop;
        Zoom();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawLine(new Vector3(minX, Camera.main.orthographicSize),
        new Vector3(maxX, Camera.main.orthographicSize));

        Gizmos.DrawLine(new Vector3(minX, -(Camera.main.orthographicSize)),
                new Vector3(maxX, -(Camera.main.orthographicSize)));

                Gizmos.DrawLine(new Vector3(minX, Camera.main.orthographicSize),
        new Vector3(minX, -Camera.main.orthographicSize));

        Gizmos.DrawLine(new Vector3(maxX, (Camera.main.orthographicSize)),
                new Vector3(maxX, -(Camera.main.orthographicSize)));
    }

    public void ResetTransforms()
    {
        playerTop = GameManager.instance.playerTop;
        playerBot = GameManager.instance.playerBot;
    }

    public void ZoomZone(float newZoom)
    {
        if(newZoom != cam.orthographicSize)
            Zoom(newZoom);
    }
}
