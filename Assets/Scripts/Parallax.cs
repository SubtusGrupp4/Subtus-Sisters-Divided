using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform cam;

    private Vector3 CamPos;

    private Vector3 ogCamPos;
    private Vector3 camChange;

    [SerializeField]
    private bool parallaxX = true;
    [SerializeField]
    private bool parallaxY = false;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main.transform;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;
        DoParallax();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DoParallax();
    }

    private void DoParallax() 
    {
        if(parallaxX) 
        {
            // the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            // changed the scale to be the z position of the parallaxed object / 10

            float parallax = (cam.position.x - CamPos.x) * (transform.position.z / 10f);

            // set a target x position which is the current position plus the parallax
            float backgroundTargetPosX = transform.position.x + parallax;

            // create a target position which is the background's current position with it's target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, transform.position.y, transform.position.z);

            // fade between current position and the target position using lerp
            transform.position = backgroundTargetPos;
        }

        if(parallaxY) 
        {
            // the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            // changed the scale to be the z position of the parallaxed object / 10

            float parallax = (cam.position.y - CamPos.y) * (transform.position.z / 10f);

            // set a target x position which is the current position plus the parallax
            float backgroundTargetPosY = transform.position.y + parallax;

            // create a target position which is the background's current position with it's target x position
            Vector3 backgroundTargetPos = new Vector3(transform.position.x, backgroundTargetPosY, transform.position.z);

            // fade between current position and the target position using lerp
            transform.position = backgroundTargetPos;
        }

        CamPos = cam.position;
    }
}
