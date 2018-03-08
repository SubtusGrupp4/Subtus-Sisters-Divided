using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionParent : MonoBehaviour {

    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Vector3 offset;
	
	void Update ()
    {
        transform.position = parent.position + offset;
	}
}
