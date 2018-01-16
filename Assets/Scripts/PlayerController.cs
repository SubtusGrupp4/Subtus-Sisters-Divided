using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField]
    private string movementAxis = "HorizontalTop";
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private string jumpAxis = "JumpTop";    // Implement with ground checking
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private float jumpHeight = 10f;

    private float horizontal;

    [Header("Physics")]
    [SerializeField]
    private bool upsideDown = false;
    [SerializeField]
    private float gravityScale = 1f;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        if(upsideDown)      // Reverse physics values if upside down
        {
            gravityScale = -gravityScale;
            jumpHeight = -jumpHeight;
        }

        rb.gravityScale = gravityScale;
    }
	
	void Update ()
    {
        horizontal = Input.GetAxisRaw(movementAxis);
        if(Input.GetKeyDown(jumpKey))
        {
            rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
        }
	}

    private void FixedUpdate()
    {
        rb.AddForce(new Vector2(horizontal * speed, 0f), ForceMode2D.Impulse);
    }
}
