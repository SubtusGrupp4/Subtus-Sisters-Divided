using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour {

	private GameObject[] ignoreObject;

	// Use this for initialization
	void Start () {
		ignoreObject = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject IO in ignoreObject)
			Physics2D.IgnoreCollision (IO.GetComponent<Collider2D> (), this.GetComponent<Collider2D>(), true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
