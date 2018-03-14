using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBoxes : MonoBehaviour
{

    [Tooltip("Changes the box's rigidbodyweight")]
    public float pushSpeed;

    private float distance;
    private GameObject box;
    private Vector2 rayLine;
    [HideInInspector]
    public bool isPulling;

    private string c1 = "_C1";
    private string c2 = "_C2";

    private string pushAndPullBox = "Pushing";

    private string tagName = "Box";

    private Controller player;
    private PlayerController playerController;

    private BasicAnimator arm;
    private BasicAnimator body;
    private float joyStickInput;


    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController.player == Controller.Player1)
        {
            pushAndPullBox += c1;
        }
        else
        {
            pushAndPullBox += c2;
        }
        distance = 1f;

        arm = playerController.armAnim;
        body = playerController.bodyAnim;

    }

    void Update()
    {
        PullOrPush();
	
        if (playerController.player == Controller.Player1)
            rayLine = new Vector2(transform.position.x, transform.position.y - 1f);
        else
            rayLine = new Vector2(transform.position.x, transform.position.y + 1.5f);

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(rayLine, Vector2.right * transform.localScale.x, distance);

        if (hit.collider != null && hit.collider.gameObject.tag == tagName && (hit.collider.GetComponent<OverEdgeFalling>() != null)
            && !playerController.inAir && (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown(pushAndPullBox)))
        {


			if (hit.collider.GetComponent<OverEdgeFalling> ().IsGrounded ()) 
			{
				isPulling = true;
				playerController.pulling = true;

				Debug.Log ("1");

				box = hit.collider.gameObject;
				box.GetComponent<Rigidbody2D> ().mass = pushSpeed;
				box.GetComponent<FixedJoint2D> ().enabled = true;
				box.GetComponent<FixedJoint2D> ().connectedBody = this.GetComponent<Rigidbody2D> ();

				Debug.Log ("2");


				Debug.Log ("3");
			}
        }
		else if ((Input.GetKeyUp(KeyCode.G) || Input.GetButtonUp(pushAndPullBox)) && isPulling || (isPulling && playerController.inAir))
		{
            isPulling = false;
            playerController.pulling = false;

            body.Pull(false);
            arm.Pull(false);

            body.Push(false);
            arm.Push(false);

            box.GetComponent<Rigidbody2D>().mass = 100;
            box.GetComponent<Rigidbody2D>().velocity = new Vector2(0, box.GetComponent<Rigidbody2D>().velocity.y);
            box.GetComponent<FixedJoint2D>().enabled = false;
        }
		if (!isPulling) 
		{
			playerController.pulling = false;

			body.Pull(false);
			arm.Pull(false);

			body.Push(false);
			arm.Push(false);
		}

    }

	public void EnableController()
	{
		GetComponent<PlayerController> ().enabled = false;
		Invoke ("DisableController", 0.5f);
	}

	public void DisableController()
	{
		GetComponent<PlayerController> ().enabled = true;
	}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(rayLine, rayLine + Vector2.right * transform.localScale.x * distance);
    }

    private void PullOrPush()
    {
       
        if (isPulling)
        {
            joyStickInput = playerController.X;


            bool isRight;

            if (box.transform.position.x - transform.position.x > 0)
            {
                isRight = true;
                
            }
            else
                isRight = false;

            Debug.Log("IsRight" + isRight);

            if (isRight && joyStickInput > 0 || !isRight && joyStickInput < 0)
            {
                body.Pull(false);
                arm.Pull(false);

                body.Push(true);
                arm.Push(true);
            }
            else if (isRight && joyStickInput < 0 || !isRight && joyStickInput > 0)
            {
                body.Pull(true);
                arm.Pull(true);

                body.Push(false);
                arm.Push(false);
            }
            else
            {
                body.Pull(false);
                arm.Pull(false);

                body.Push(false);
                arm.Push(false);
            }

        }
			
    }
}
