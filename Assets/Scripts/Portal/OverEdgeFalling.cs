using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverEdgeFalling : MonoBehaviour
{
	private GameObject[] ignoreObject;

	private Collider2D col;
	private SpriteRenderer sp;
	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D> ();
		col = GetComponent<Collider2D> ();
		sp = GetComponent<SpriteRenderer> ();
		ignoreObject = GameObject.FindGameObjectsWithTag ("Player");
	}

	void Update()
	{
		if (!IsGrounded ()) 
		{
			rb.mass = 100;
			if (GetComponent<FixedJoint2D> ().enabled) {
				rb.velocity = new Vector2 (0, rb.velocity.y);
			}
			GetComponent<FixedJoint2D> ().enabled = false;
			foreach (GameObject IO in ignoreObject)
				Physics2D.IgnoreCollision (IO.GetComponent<Collider2D> (), this.col, true);
		} else 
		{
			foreach (GameObject IO in ignoreObject)
				Physics2D.IgnoreCollision (IO.GetComponent<Collider2D> (), this.col, false);
		}
		
	}

	public bool IsGrounded()
	{
		Vector2 leftPosition = new Vector2(transform.position.x - sp.bounds.size.x/2 -0.03f, transform.position.y);
		Vector2 rightPosition = new Vector2(transform.position.x + sp.bounds.size.x/2 + 0.03f, transform.position.y);
		Vector2 direction = Vector2.down * transform.localScale.y;

		float distance = 0.6f;
		bool checkLeftRay;
		bool checkRightRay;

		RaycastHit2D hit = Physics2D.Raycast (leftPosition, direction, distance);
		RaycastHit2D hit2 = Physics2D.Raycast (rightPosition, direction, distance);
		if (hit.collider != null) {
			
			checkLeftRay = true;
		} else {
			checkLeftRay = false;
		}

		if (hit2.collider != null) {
			
			checkRightRay = true;
		} else {
			checkRightRay = false;
		}


		if (!checkRightRay && !checkLeftRay)
		{
			return false;
		}


		return true;
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawLine (transform.position, (Vector2)transform.position + Vector2.down * transform.localScale.y * 0.6f);
	}


}