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
        if (collision.tag == "Player" &&cc.zoomTo != newZoom && !GameManager.instance.onePlayerDead)
            cc.ZoomZone(newZoom);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.7f);
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
        Gizmos.color = new Color(1f, 0.6f, 0.9f);
        Gizmos.DrawWireCube(transform.position, new Vector3((newZoom * 2f / 9f) * 16f, newZoom * 2f));
    }
}
