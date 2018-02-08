using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : PortalBehaviour 
{

	[Header ("Scripts")]
	public MonoBehaviour enemyScript;
	public MonoBehaviour friendlyScript;

	[Header("AnimatorController")]
	public RuntimeAnimatorController original;
	public RuntimeAnimatorController reversed;

	[Header("Colliders")]
	public Collider2D originalCollider;
	public Collider2D reversedCollider;

	private int q = 0;
	private bool enabler= true;

	void Invoker()
	{
		originalCollider.enabled = !originalCollider.enabled;
		enemyScript.enabled = !enabler;
		reversedCollider.enabled = !reversedCollider.enabled;
		friendlyScript.enabled = enabler;
		enabler = !enabler;

	}

	void ReverseComponents()
	{
		q++;

		if (gameObject.GetComponent<Rigidbody2D>().gravityScale < 0) 
		{
			this.GetComponent<Animator> ().runtimeAnimatorController = original as RuntimeAnimatorController;
			transform.localScale = new Vector3 (1f, 1f, 1f);
		} else 
		{
			this.GetComponent<Animator> ().runtimeAnimatorController = reversed as RuntimeAnimatorController;
			transform.localScale = new Vector3 (1f, -1f, 1f);
		}
		//linear drag -- solution?
		gameObject.GetComponent<Rigidbody2D> ().gravityScale *= -1;
	}

	public override void OnPortalContact()
	{
		Invoke ("Invoker", 0.1f);
		ReverseComponents ();
	}
}
