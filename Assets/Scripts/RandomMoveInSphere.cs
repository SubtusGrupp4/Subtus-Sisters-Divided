using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveInSphere : MonoBehaviour {

	public float frequency = 0.1f;
    public float strength = 1f;

    private Vector3 startPos;
	[SerializeField]
    private Vector2 randomDir;
	private float timeSinceRandomRefresh = 1f;

	private Rigidbody2D rb;

	void Start () 
	{
		startPos = transform.position;
		rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate()
	{
		rb.AddForce(startPos - transform.position * strength);

		if(timeSinceRandomRefresh >= 1.5f) 
		{
			randomDir = Random.insideUnitSphere;
			timeSinceRandomRefresh = 0f;
		} 
		else 
		{
			timeSinceRandomRefresh += Time.deltaTime / frequency;
			rb.AddForce(new Vector2(randomDir.x, randomDir.y) * strength);
		}
	}
}
