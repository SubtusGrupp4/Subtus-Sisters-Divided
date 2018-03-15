using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnRadius : MonoBehaviour {

    private Vector3 startPosition;

    [SerializeField]
    private float radius = 15f;

	void Start ()
    {
        startPosition = transform.position;
	}
	
	void Update ()
    {
		if(Vector3.Distance(transform.position, startPosition) > radius)
        {
            transform.position = startPosition;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.75f, 0f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
