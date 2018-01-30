using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationBomb : MonoBehaviour 
{
	[Header("Attributes")]
	public int range;
	public int pullForce;
	public float duration;
	public float timeBeforeBombDisappear;
	[Header("tags from objects to pull")]
	[GiveTag]
	public string[] tagNames;

	private List<GameObject> pullObjects;
	private Rigidbody2D rb;
	private bool buttonPressed;
	private bool targetting;
	private bool gravitationActivated;
	private int decreaseGravity;

	void Start () 
	{
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
	}

	void FixedUpdate()
	{
		GravitationPull ();
	}

	void ActivateGravitationBomb()
	{
		if (Input.GetKey (KeyCode.O) && buttonPressed) 
		{
			buttonPressed = false;
			targetting = true;
			gravitationActivated = true;
			rb.bodyType = RigidbodyType2D.Static;
			Destroy (gameObject, duration);
		}
		if(Input.GetKeyUp(KeyCode.O))
		{
			Destroy (gameObject);
		}
		Destroy (gameObject, timeBeforeBombDisappear);
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
			toPull.GetComponent<Rigidbody2D> ().velocity = new Vector2(toPull.GetComponent<Rigidbody2D>().velocity.x/integer, toPull.GetComponent<Rigidbody2D>().velocity.y/integer);
		}
	}

	void onDestroy()
	{
		if (gravitationActivated) 
		{
			ResetGravity (10);
		}
	}

}


