using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPortal : PortalBehaviour 
{
	private Rigidbody2D rb;
	private bool checkState = true;
    private int lengthToPortal;

    [SerializeField]
    private bool flipScale = true;
    
	void Start ()
    {
		rb = GetComponent<Rigidbody2D>();
	}

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
            lengthToPortal = (int)transform.position.y;
    }

	public override void OnPortalContact()
	{
		if (rb != null)
		{
			checkState = !checkState;
            if(flipScale)
			    transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
			rb.gravityScale = -rb.gravityScale;

                float gravity = -9.81f;
                float power;
                float offSet = 0.7f;

                // U^2  = sluthastigheten ^2 - 2 * A (gravity) s (height / lenght)
                power = 0 - (2 * (gravity * (Mathf.Abs(lengthToPortal) + offSet)));
                // U = sqrt U^2 
                power = Mathf.Sqrt(power);
                if(Debug.isDebugBuild)
                    Debug.Log("PORTAL RUNNING YES YES   " + rb.velocity.y);
                
                rb.velocity = new Vector2(rb.velocity.x, rb.gravityScale * power);
        }
	}
}
