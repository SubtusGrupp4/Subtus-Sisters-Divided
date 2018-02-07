using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour {


	public RuntimeAnimatorController anim;
	public bool state = true;



	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Debug.Log (2);
		ReverseComponents ();
	}

	void ReverseComponents()
	{
		if (state) {
			this.GetComponent<Animator> ().runtimeAnimatorController = anim as RuntimeAnimatorController;

		}
	}
}
