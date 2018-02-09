using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGravityBomb : MonoBehaviour 
{
	public GameObject gravityBomb;
	public GameObject shockwaveBomb;
	private GameObject fireBomb;
	private GameObject clone;

	[SerializeField]
	private float speed;
	[SerializeField]
	private float cooldown;
	[SerializeField]
	[Range(0f, 0.75f)]
	private float sensitivity;

	private Vector2 direction;
	private bool reloaded;
	private float setCooldown;
	private int flipValue;

	//input Manager
	private string controllerCode;

	private string controllerOne = "_C1";
	private string controllerTwo = "_C2";

	private string rightXAxis = "Horizontal_Fire";
	private string rightYAxis = "Vertical_Fire";

    public GameObject arm;
    public GameObject throwArm;

	[SerializeField]
    private BasicAnimator playerAnim;
    private BasicAnimator armAnim;
    private BasicAnimator throwArmAnim;

    private SpriteRenderer armSprite;
    private SpriteRenderer throwArmSprite;

    private bool animationPlaying;

    [SerializeField]
    private PlayerController playerController;

    private bool isFlipped = false;


    void Start()
	{
        armAnim = arm.GetComponent<BasicAnimator>();
        throwArmAnim = throwArm.GetComponent<BasicAnimator>();

        armSprite = arm.GetComponent<SpriteRenderer>();
        throwArmSprite = throwArm.GetComponent<SpriteRenderer>();

        reloaded = true;
		setCooldown = cooldown;
		if (playerController.Player == Controller.Player1) 
		{
			controllerCode = controllerOne;
			fireBomb = shockwaveBomb;
			flipValue = 1;
		} 
		else 
		{
			controllerCode = controllerTwo;
			fireBomb = gravityBomb;
			flipValue = -1;
		}
		rightXAxis += controllerCode;
		rightYAxis += controllerCode;
	}

	void Update()
	{
		GetDirection();
		Fire(direction, fireBomb);

        if(animationPlaying)
        {          
            if(throwArm.GetComponent<Animator>().GetBool(BasicAnimator.animAttack) == false)
            {
                animationPlaying = false;
                throwArmSprite.enabled = false;
                armSprite.enabled = true;
                if (isFlipped)
                {
                    Debug.Log("Flip back!");
                    playerAnim.Flip();
                    isFlipped = false;
                }
            } 
        }		
	}


	void Fire(Vector2 Direction, GameObject fireObj)
	{
		if (((Mathf.Abs(Direction.x) > sensitivity && reloaded) || (Mathf.Abs(Direction.y) > sensitivity && reloaded) || Input.GetKeyDown(KeyCode.P)) && clone == null) 
		{	
			if (Input.GetKeyDown (KeyCode.P)) 
				Direction = new Vector2 (1, 1);

            /* Moonwalking
			if (Direction.x < 0) 
			{
				transform.parent.localScale = new Vector3 (-1, transform.parent.localScale.y, transform.parent.localScale.z);
			} else 
			{
				transform.parent.localScale = new Vector3 (1, transform.parent.localScale.y, transform.parent.localScale.z);
			}
			*/

            AnimationAttacking(Direction);

            clone = Instantiate(fireObj, transform.position, Quaternion.identity);
			clone.GetComponent<Rigidbody2D>().AddForce (Direction.normalized * speed);
			clone.GetComponent<Rigidbody2D>().gravityScale = flipValue;
			reloaded = false;
			setCooldown = cooldown;
			direction = Vector2.zero;
		}

		Reload();
	}

    void AnimationAttacking(Vector2 dir)
    {
        throwArmSprite.enabled = true;
        armSprite.enabled = false;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        throwArm.transform.rotation = new Quaternion(0, 0, rot.z, rot.w);
        throwArmAnim.Attack();

        if (dir.x < 0 && !isFlipped)
        {
            Debug.Log("Flip!");
            playerAnim.Flip();
            isFlipped = true;
        }

        animationPlaying = true;
    }

    void GetDirection()
	{
		float x = Input.GetAxisRaw(rightXAxis);
		float y = Input.GetAxisRaw(rightYAxis);

		direction = new Vector2(x, y);
	}


	void Reload()
	{
		if(clone == null && !reloaded && setCooldown > 0f)
			setCooldown -= Time.deltaTime;

        if (setCooldown <= 0f && !reloaded)
        {
            setCooldown = cooldown;
            reloaded = true;
        }
    }
}
