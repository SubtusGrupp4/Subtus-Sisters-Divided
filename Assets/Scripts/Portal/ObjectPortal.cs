using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPortal : PortalBehaviour 
{
	public Sprite original;
	public Sprite reversed;

	private Rigidbody2D rb;
	private bool checkState = true;
	private bool onPortal;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (onPortal) {
			if (transform.position.y > 10) {
				rb.velocity = new Vector2(0, -1);
			} else if (transform.position.y < -10) {
				rb.velocity = new Vector2(0, 1);
			}
		}
		if (rb.velocity == Vector2.zero) {
			onPortal = false;
		}
	}

	void ChangeSprite(){
		if (!CompareTag("GravitationBomb")) {
			if (checkState) {
				this.GetComponent<SpriteRenderer> ().sprite = original;
			} else {
				this.GetComponent<SpriteRenderer> ().sprite = reversed;
			}
		}
	}

	public override void OnPortalContact()
	{
		onPortal = true;
		if (rb != null)
		{
			checkState = !checkState;
			ChangeSprite ();
			rb.gravityScale = -rb.gravityScale;

		}
	}

}
