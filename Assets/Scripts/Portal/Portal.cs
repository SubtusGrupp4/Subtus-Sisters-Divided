using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour 
{
	public GameObject game;
	public GameObject clone;
	public bool test1 = false;
	public float test2;
	
	void OnTriggerExit2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PortalBehaviour>() != null) 
		{
			if (test2 > otherObj.gameObject.transform.position.y/2 && otherObj.gameObject.transform.position.y < 0) {
				otherObj.GetComponent<Rigidbody2D> ().gravityScale *= -1;
			}
			if(test2 < otherObj.gameObject.transform.position.y /2 && otherObj.gameObject.transform.position.y > 0){
				otherObj.GetComponent<Rigidbody2D> ().gravityScale *= -1;
			}
			otherObj.GetComponent<PortalBehaviour> ().OnPortalContact ();
		}
	}

	void OnTriggerEnter2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PlayerPortal> () != null) {
			otherObj.GetComponent<PlayerPortal> ().OnPortalContact ();
		} else {
			clone = Instantiate (game, otherObj.gameObject.transform.position, Quaternion.identity);
			test2 = otherObj.gameObject.transform.position.y;
		}
	}


}
