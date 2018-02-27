using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverEdgeFalling : MonoBehaviour
{
	public float constantSpeed = 2f;

	public LayerMask layer;

	private Collider2D col;
	private SpriteRenderer sp;
	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D> ();
		col = GetComponent<BoxCollider2D> ();
		sp = GetComponent<SpriteRenderer> ();

	}

	void Update()
	{
		if (!IsGrounded ()) {
			rb.mass = 100;
			if (GetComponent<FixedJoint2D> ().enabled) {
				rb.velocity = new Vector2 (0, rb.velocity.y);
			}
			GetComponent<FixedJoint2D> ().enabled = false;
		}
		
	}

	public bool IsGrounded()
	{
		Vector2 leftPosition = new Vector2(transform.position.x - sp.bounds.size.x/2, transform.position.y);
		Vector2 rightPosition = new Vector2(transform.position.x + sp.bounds.size.x/2, transform.position.y);
		Vector2 direction = Vector2.down * transform.localScale.y;

		float distance = 1.0f;
		bool checkLeftRay;
		bool checkRightRay;

		RaycastHit2D hit = Physics2D.Raycast (leftPosition, direction, distance, layer);
		RaycastHit2D hit2 = Physics2D.Raycast (rightPosition, direction, distance, layer);
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

}