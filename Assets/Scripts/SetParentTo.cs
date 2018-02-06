using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetParentTo : MonoBehaviour {

    [SerializeField]
    private Transform parent;
    void Update () {
		if(parent != null)
        	transform.parent = parent;
    }
}
