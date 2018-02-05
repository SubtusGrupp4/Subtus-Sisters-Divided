using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called rant


	void OnTriggerExit2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PortalBehaviour>() != null) {
			otherObj.GetComponent<PortalBehaviour> ().OnPortalContact ();
		}
	}


}
