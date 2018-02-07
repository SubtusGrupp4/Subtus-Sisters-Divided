using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

	void OnTriggerExit2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PortalBehaviour>() != null) {
			otherObj.GetComponent<PortalBehaviour> ().OnPortalContact ();
		}
	}

	void OnTriggerEnter2D(Collider2D otherObj){
		if (otherObj.gameObject.GetComponent<PlayerPortal> () != null) {
			otherObj.GetComponent<PlayerPortal> ().OnPortalContact ();
		}
	}


}
