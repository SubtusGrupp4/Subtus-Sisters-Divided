using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomZone : MonoBehaviour
{
    [SerializeField]
    private float newZoom = 15f;
    private CameraController cc;

    void Start ()
    {
        cc = Camera.main.GetComponent<CameraController>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" &&cc.zoomTo != newZoom)
            cc.ZoomZone(newZoom);
    }
}
