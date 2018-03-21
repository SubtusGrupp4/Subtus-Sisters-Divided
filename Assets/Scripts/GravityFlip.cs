using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityFlip : MonoBehaviour {

    Rigidbody2D rb;
    [SerializeField]
    private bool flipGravity = true;
    [SerializeField]
    private bool flipLocalScale = true;

	// Use this for initialization
	void Start () {
        if (transform.position.y < 0f)
        {
            if (flipGravity)
            {
                rb = GetComponent<Rigidbody2D>();
                rb.gravityScale *= -1f;
            }
            if (flipLocalScale)
                transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1f);
        }
	}

    public void SetGravity(float yPos)
    {
        if (yPos > 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            rb.gravityScale = Mathf.Abs(rb.gravityScale);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
            rb.gravityScale = -Mathf.Abs(rb.gravityScale);
        }
    }
}
