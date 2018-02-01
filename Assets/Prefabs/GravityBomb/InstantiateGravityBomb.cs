using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGravityBomb : MonoBehaviour 
{
	public GameObject gravityBomb;
	private GameObject clone;

	[SerializeField]
	private float speed;
	[SerializeField]
	private float cooldown;
	[SerializeField]
	[Range(0f, 0.75f)]
	private float sensitivity;
	private Vector2 direction;
	private bool reload;
	private float setCooldown;

	private string rightXAxis;
	private string rightYAxis;


	void Start()
	{
		reload = true;
		setCooldown = cooldown;
		rightXAxis = "Horizontal_Fire_C1";
		rightYAxis = "Vertical_Fire_C1";
	}

	void Update()
	{
		GetDirection ();
		Fire (direction, gravityBomb);
		
	}


	void Fire(Vector2 Direction, GameObject fireObj)
	{
		if (((Mathf.Abs(Direction.x) > sensitivity && reload) || (Mathf.Abs(Direction.y) > sensitivity && reload)) && clone == null) 
		{	
			
			if (Direction.x < 0) 
			{
				transform.parent.localScale = new Vector3 (-1, transform.parent.localScale.y, transform.parent.localScale.z);
			} else 
			{
				transform.parent.localScale = new Vector3 (1, transform.parent.localScale.y, transform.parent.localScale.z);
			}

			//Debug.Log (Direction);
			clone = (GameObject)Instantiate (fireObj, transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Rigidbody2D> ().AddForce (Direction.normalized * speed);
			reload = false;
			setCooldown = cooldown;
			direction = Vector2.zero;
		}
		Reload ();
	}

	void GetDirection()
	{
		float x = Input.GetAxisRaw (rightXAxis);
		float y = Input.GetAxisRaw (rightYAxis);

		direction = new Vector2(x, y);

	}


	void Reload()
	{
		setCooldown -= Time.deltaTime;
		if (setCooldown < 0 && !reload) 
		{
			reload = true;
		}
	}

}
