using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    FollowingBoth, FollowingOne, Frozen, Moving
}

public class CameraController : MonoBehaviour
{
    public CameraState State;

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
    public Vector2 followPos;
    [SerializeField]
    private float camDistance;

    private float camWait = 0f;

    private Transform target;

    [Header("Zooming")]
    [SerializeField]
    private float yZoomOffset;
    [SerializeField]
    private float minZoom;
    [SerializeField]
    private float maxZoom;
    [SerializeField]
    private float zoomTime = 1f;
    private float currentZoomTime;
    private bool zoomIn = false;
    private float startZoom;

    private void Start()
    {
        startFollowSpeed = followSpeed;
        State = CameraState.FollowingBoth;

        currentZoomTime = zoomTime;
    }

    private void Update()
    {
        if(State == CameraState.FollowingBoth)
        {
            if(zoomIn)
            {
                currentZoomTime = 0f;
                zoomIn = false;
                startZoom = GetComponent<Camera>().orthographicSize;
            }

            currentZoomTime += Time.deltaTime;
            if (currentZoomTime > zoomTime)
            {
                currentZoomTime = zoomTime;
            }

            float t = currentZoomTime / zoomTime;
            t = t * t * (3f - 2f * t);

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoom, maxZoom, t);

            transform.position = new Vector3(transform.position.x, 0f, -10f);
        }
        else if(State == CameraState.FollowingOne)
        {
            if (!zoomIn)
            {
                currentZoomTime = 0f;
                zoomIn = true;
                startZoom = GetComponent<Camera>().orthographicSize;
            }

            currentZoomTime += Time.deltaTime;
            if (currentZoomTime > zoomTime)
            {
                currentZoomTime = zoomTime;
            }

            float t = currentZoomTime / zoomTime;
            t = t * t * (3f - 2f * t);

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoom, minZoom, t);

            if (target == playerTop)
                transform.position = new Vector3(transform.position.x, yZoomOffset, -10f);
            else
                transform.position = new Vector3(transform.position.x, -yZoomOffset, -10f);
        }
    }

    private void FixedUpdate()
    {
        //if (GameObject.Find("DialogueManager") == null || !DialogueManager.instance.moveCamera && !DialogueManager.instance.freezeCamera)
        if (State == CameraState.FollowingBoth)
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
            camDistance = target.position.x - transform.position.x;
            //float camDistanceY = target.position.y - transform.position.y;

            if (camDistance > 0.1f)
                transform.Translate(new Vector2((camDistance - deadZone) / followSpeed, 0.0f), Space.Self);      // Move Right
            else if (camDistance < -0.1f)
                transform.Translate(new Vector2((camDistance + deadZone) / followSpeed, 0.0f), Space.Self);      // Move Left

            //transform.Translate(new Vector2(0f, camDistanceY / followSpeed));

            //GetComponent<Camera>().orthographicSize = 5f;

        }
        //else
        else if(State == CameraState.Moving)
        {
            camDistance = followPos.x - transform.position.x;

            transform.Translate(new Vector2(camDistance / followSpeed, 0.0f), Space.Self);  // Move Right

            // Camera Move Finished
            if (camDistance < 0.1f)
            {
                StartCoroutine(CameraWait());
            }
        }
    }

    public void SetCameraState(CameraState State)
    {
        this.State = State;
    }

    public void SetCameraState(CameraState State, Transform playerTarget)
    {
        this.State = State;
        Debug.Log(playerTarget);
        if (playerTarget == playerTop)
            target = playerBot;
        else
            target = playerTop;
        Debug.Log(target);
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
