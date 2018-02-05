using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PortalBehaviour : MonoBehaviour {

	protected string portalTag = "Portal";


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void OnPortalContact()
	{
		if (this.GetComponent<PlayerController> () != null) 
		{
			GetComponent<PlayerController> ().Die ();
		}
	} 

}
