using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBoxes : MonoBehaviour
{

    [Tooltip("Changes the box's rigidbodyweight")]
    public float pushSpeed;
	[Tooltip("How long is the character going to stop when pushing box over edge")]
	[Range(0, 2)]
	public float delayTime;
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

    public bool flip = false;

    private FMODEmitter emitter;

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

        emitter = GetComponent<FMODEmitter>();
    }

    void Update()
    {
        PullOrPush();

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(rayLine, Vector2.right * transform.localScale.x, distance);

        if (!flip)
        {
            if (playerController.player == Controller.Player1)
                rayLine = new Vector2(transform.position.x, transform.position.y - 1f);
            else
                rayLine = new Vector2(transform.position.x, transform.position.y + 1.5f);
        }
        else
        {
            if (playerController.player == Controller.Player2)
                rayLine = new Vector2(transform.position.x, transform.position.y - 1.5f);
            else
                rayLine = new Vector2(transform.position.x, transform.position.y + 1f);
        }

        if (hit.collider != null && hit.collider.gameObject.tag == tagName && (hit.collider.GetComponent<OverEdgeFalling>() != null)
            && !playerController.inAir)
        {
            if (hit.transform.GetComponent<DisplayIconTrigger>() != null && !isPulling && hit.transform.GetComponent<OverEdgeFalling>().IsGrounded())
                hit.transform.GetComponent<DisplayIconTrigger>().SetShowIcon(true);

            if (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown(pushAndPullBox))
            {
                if (hit.collider.GetComponent<OverEdgeFalling>().IsGrounded())
                {
                    box = hit.collider.gameObject;
                    StartDragging();

                    if (hit.transform.GetComponent<DisplayIconTrigger>() != null)
                        hit.transform.GetComponent<DisplayIconTrigger>().SetShowIcon(false);
                }
            }
        }
        else if ((Input.GetKeyUp(KeyCode.G) || Input.GetButtonUp(pushAndPullBox)) && isPulling || (isPulling && playerController.inAir))
        {
            StopDragging();
        }
        if(!isPulling)
		{
			playerController.pulling = false;
            emitter.Stop();

			body.Pull(false);
			arm.Pull(false);

			body.Push(false);
			arm.Push(false);
		}

    }

    public void Flip()
    {
        flip = !flip;  
    }

    private void StartDragging()
    {
        isPulling = true;
        playerController.pulling = true;

        box.GetComponent<Rigidbody2D>().mass = pushSpeed;
        box.GetComponent<FixedJoint2D>().enabled = true;
        box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();

        if (box.GetComponent<ReturnRadius>() != null)
            box.GetComponent<ReturnRadius>().draggedBy = transform;
    }

    public void StopDragging()
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
        emitter.Stop();
    }

	public void EnableController()
	{
		GetComponent<PlayerController> ().enabled = false;
		Invoke ("DisableController", 1f);
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

                emitter.Play();
            }
            else if (isRight && joyStickInput < 0 || !isRight && joyStickInput > 0)
            {
                body.Pull(true);
                arm.Pull(true);

                body.Push(false);
                arm.Push(false);

                emitter.Play();
            }
            else
            {
                body.Pull(false);
                arm.Pull(false);

                body.Push(false);
                arm.Push(false);

                emitter.Stop();
            }

        }


    }
}
