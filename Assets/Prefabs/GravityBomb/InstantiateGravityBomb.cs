using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGravityBomb : MonoBehaviour 
{

	public GameObject gravityBomb;
	public float speed;
	public float cooldown;

	private Vector2 direction;
	private bool reload;
	private float setCooldown;


	void Start()
	{
		direction = new Vector2 (1,1);
		reload = true;
		setCooldown = cooldown;
	}

	void Update()
	{
		
			Fire (direction, gravityBomb);
		
	}


	void Fire(Vector2 Direction, GameObject fireObj)
	{
		if (Input.GetKeyDown (KeyCode.P) && reload) {	
			GameObject clone = (GameObject)Instantiate (fireObj, transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Rigidbody2D> ().AddForce (Direction * speed);
			reload = false;
			setCooldown = cooldown;
		}
		Reload ();
	}



	void Reload()
	{
		setCooldown -= Time.deltaTime;
		if (setCooldown < 0 && !reload) {
			reload = true;
		}
	}

}
