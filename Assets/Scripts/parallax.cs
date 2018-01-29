using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{

    [SerializeField]
    [Range(-1, 1)]
    private float trackSpeed;


    private Transform cam;

    private Vector3 offSet;

    private Vector3 CamPos;

    private Vector3 ogCamPos;
    private Vector3 camChange;

    // Use this for initialization
    void Start()
    {
        cam = FindObjectOfType<Camera>().transform;
        offSet = transform.position - cam.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {

        if (CamPos != cam.transform.position)
        {
            // the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            float parallax = (cam.position.x - CamPos.x) * trackSpeed;

            // set a target x position which is the current position plus the parallax
            float backgroundTargetPosX = transform.position.x + parallax;

            // create a target position which is the background's current position with it's target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, transform.position.y, transform.position.z);

            // fade between current position and the target position using lerp
            transform.position = backgroundTargetPos;

            CamPos = cam.position;
        }

    }
}
