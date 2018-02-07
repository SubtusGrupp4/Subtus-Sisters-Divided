using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FindParent : MonoBehaviour {

    public Transform parent;

	// Use this for initialization
	void Update () {
        parent = transform.parent;
        parent.hideFlags = HideFlags.None;
	}
}
