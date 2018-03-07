using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateChildOnPlay : MonoBehaviour {

    [SerializeField]
    private GameObject childPrefab;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Material darkAdd;

    // Use this for initialization
    void Start () {
        Transform child = Instantiate(childPrefab, transform).transform;
        child.localPosition = offset;
        if(child.position.y < 0f)
            child.GetComponent<SpriteRenderer>().material = darkAdd;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
