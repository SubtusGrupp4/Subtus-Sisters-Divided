using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPortal : PortalBehaviour 
{
	public Sprite original;
	public Sprite reversed;


	private Rigidbody2D rb;
	private bool checkState = true;
	private bool onPortal;

    private int lengthToPortal;
    

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (rb.velocity == Vector2.zero) {
			onPortal = false;
		}

    
        
    }


    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
        {
            lengthToPortal = (int)transform.position.y;
        }
    }

    void ChangeSprite(){
		if (!CompareTag("GravitationBomb") || !CompareTag("Pickup")) {
			if (checkState) {
				this.GetComponent<SpriteRenderer> ().sprite = original;
			} else {
				this.GetComponent<SpriteRenderer> ().sprite = reversed;
			}
		}
	}

	public override void OnPortalContact()
	{
		onPortal = true;
		if (rb != null)
		{
			checkState = !checkState;
			ChangeSprite ();
			transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
			rb.gravityScale = -rb.gravityScale;


            
                float gravity = -9.81f;
                float power;
                float offSet = 0.7f;

                // U^2  = sluthastigheten ^2 - 2 * A (gravity) s (height / lenght)
                power = 0 - (2 * (gravity * (Mathf.Abs(lengthToPortal) + offSet)));
                // U = sqrt U^2 
                power = Mathf.Sqrt(power);
                Debug.Log("PORTAL RUNNING YES YES   " + rb.velocity.y);
                
                rb.velocity = new Vector2(rb.velocity.x, rb.gravityScale * power);
            

        }
	}

}
