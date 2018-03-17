using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomZone : MonoBehaviour
{
    private CameraController cc;
    [SerializeField]
    private float newZoom = 15f;
    [SerializeField]
    private bool restoreOnExit = false;
    private float initialZoom;

    private int inZone = 0;

    void Start ()
    {
        cc = Camera.main.GetComponent<CameraController>();
        initialZoom = cc.maxZoom;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            initialZoom = cc.maxZoom;
            inZone++;
            if (inZone == 2)
                cc.ZoomZone(newZoom);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (restoreOnExit && collision.tag == "Player")
        {
            inZone--;
            if(inZone == 0)
                cc.ZoomZone(initialZoom);
        }
    }
}
