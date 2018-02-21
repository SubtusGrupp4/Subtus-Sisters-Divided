using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PortalBehaviour : MonoBehaviour 
{

	public virtual void OnPortalContact()
	{
		if (this.GetComponent<PlayerController> () != null) 
		{
            // This is handled in PlayerController
			//GetComponent<PlayerController> ().Die ();
		}

	} 



}
