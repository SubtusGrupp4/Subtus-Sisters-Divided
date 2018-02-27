using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateChildOnPlay : MonoBehaviour {

    [SerializeField]
    private GameObject childPrefab;
    [SerializeField]
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        Transform child = Instantiate(childPrefab, transform).transform;
        child.localPosition = offset;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
