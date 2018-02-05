using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : PortalBehaviour {

	public GameObject reversedObject;
	private GameObject clone;
	private float timer = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer < 0) {
			GetComponent<Collider2D> ().enabled = true;
		}
	}

	public override void OnPortalContact(){


		clone = Instantiate (reversedObject, transform.position, Quaternion.identity) as GameObject;

		Rigidbody2D cloneGravity = clone.GetComponent<Rigidbody2D> ();
		cloneGravity.velocity = this.GetComponent<Rigidbody2D> ().velocity;
		clone.GetComponent<Collider2D> ().enabled = false;
		cloneGravity.gravityScale *= -this.GetComponent<Rigidbody2D>().gravityScale;
		Destroy (gameObject);
	}
}
