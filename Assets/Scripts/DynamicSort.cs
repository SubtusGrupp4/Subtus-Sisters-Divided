using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSort : MonoBehaviour
{
	// Update is called once per frame
	void Update () {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.x);
	}
}
