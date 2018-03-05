using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{

    private Camera myCam;
    private GameManager gM;

    private GameObject pTop;
    private GameObject pBot;

    private Vector3 camPos;

    float height;
    float width;

    // Use this for initialization
    void Start()
    {
        myCam = FindObjectOfType<CameraController>().gameObject.GetComponent<Camera>();
        gM = FindObjectOfType<GameManager>();

        pTop = gM.playerTop.transform.gameObject;
        pBot = gM.playerBot.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        FetchCameraInfo();

        ClampObject(pTop);
        ClampObject(pBot);
    }

    private void FetchCameraInfo()
    {
        height = 2f * myCam.orthographicSize;
        width = height * myCam.aspect;

        camPos = myCam.transform.position;
    }

    public void ClampObject(GameObject obj)
    {
        Vector3 pos = obj.transform.position;

        float X = obj.transform.position.x;
        pos.x = Mathf.Clamp(X, myCam.transform.position.x - (width * 0.5f), myCam.transform.position.x + (width * 0.5f));
        obj.transform.position = pos;
    }
}
