using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationBomb : MonoBehaviour 
{
	
	[Header("Attributes")]
	[SerializeField]
	private int range;
	[SerializeField]
	private int pullForce;
	[SerializeField]
	private float duration;
	[SerializeField]
	private float deActivate;
	[Header("tags from objects to pull")]
	[GiveTag]
	[SerializeField]
	private string[] tagNames;

	private List<GameObject> pullObjects;
	private Rigidbody2D rb;
	private bool buttonPressed;
	private bool targetting;
	private bool gravitationActivated;
	private int decreaseGravity;
	private string fireBomb;

	void Start () 
	{
		fireBomb = "Fire_Gravitation_Bomb";
		rb = GetComponent<Rigidbody2D> ();
		pullObjects = new List<GameObject> ();
		buttonPressed = true;
		gravitationActivated = false;
		targetting = false;
		decreaseGravity = 0;
	}

	void Update () 
	{
		ActivateGravitationBomb ();
		TargetInRange ();
		TargetOutsideRange ();
		if (!buttonPressed) {
			duration -= Time.deltaTime;
			if (duration < 0) {
				ResetGravity (0);
				Destroy (gameObject);
			}
		}
	}

	void FixedUpdate()
	{
		GravitationPull ();
	}

	void ActivateGravitationBomb()
	{
		deActivate -= Time.deltaTime;
		if (Input.GetButton (fireBomb) && buttonPressed) 
		{
			buttonPressed = false;
			targetting = true;
			gravitationActivated = true;
			rb.bodyType = RigidbodyType2D.Static;
		}
		if(Input.GetButtonUp (fireBomb))
		{
			ResetGravity (0);
			Destroy (gameObject);
		}
		if (deActivate < 0) 
		{
			Destroy (gameObject);
		}
	}

	void TargetInRange()
	{
		if (targetting) 
		{
			foreach (string tag in tagNames) 
			{
				GameObject[] o = GameObject.FindGameObjectsWithTag (tag);
				foreach (GameObject targetObjects in o) 
				{
					float inRange = Vector3.Distance (transform.position, targetObjects.transform.position);
					if (range > inRange) 
					{
						pullObjects.Add (targetObjects);
					}
				}
			}
			targetting = false;
		}
	}

	void TargetOutsideRange()
	{
		for (int i = 0; i < pullObjects.Count; i++) 
		{
			float inRange = Vector3.Distance (transform.position, pullObjects[i].transform.position);
			if (range < inRange) 
			{
				pullObjects.Remove (pullObjects[i]);
			}
		}
	}

	void GravitationPull()
	{
		if (gravitationActivated) 
		{
			
			foreach (GameObject toPull in pullObjects) 
			{
				float distanceSquared = Mathf.Sqrt (Vector3.Distance (transform.position, toPull.transform.position));
				float force = pullForce*(rb.mass* toPull.GetComponent<Rigidbody2D>().mass)/ distanceSquared;

				Vector2 distance = transform.position - toPull.transform.position;
				Vector2 forceDirection = (transform.position - toPull.transform.position).normalized;
				Vector2 forceVector = forceDirection * force;
				if (distance.y < 1 && distance.x < 1) 
				{
					decreaseGravity++;
					toPull.GetComponent<Rigidbody2D> ().AddForce(forceVector / -decreaseGravity);
				}
				toPull.GetComponent<Rigidbody2D>().AddForce (forceVector);
			}
		}
	}

	void ResetGravity(int integer)
	{
		foreach (GameObject toPull in pullObjects) 
		{
			toPull.GetComponent<Rigidbody2D> ().velocity = new Vector2(toPull.GetComponent<Rigidbody2D>().velocity.x*integer, toPull.GetComponent<Rigidbody2D>().velocity.y*integer);
		}
	}



}


