using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPortal : PortalBehaviour 
{

	private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnPortalContact()
	{
		if (rb != null)
		{
			rb.gravityScale = -rb.gravityScale;
			rb.drag += 0.1f;
		}
	}

}
