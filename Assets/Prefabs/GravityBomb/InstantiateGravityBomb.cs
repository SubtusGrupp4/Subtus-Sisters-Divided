﻿using System.Collections;
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
	private bool reload;
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

    private BasicAnimator armAnim;
    private BasicAnimator throwArmAnim;

    private SpriteRenderer armSprite;
    private SpriteRenderer throwArmSprite;

    private bool animationPlaying;


	void Start()
	{
        armAnim = arm.GetComponent<BasicAnimator>();
        throwArmAnim = throwArm.GetComponent<BasicAnimator>();

        armSprite = arm.GetComponent<SpriteRenderer>();
        throwArmSprite = throwArm.GetComponent<SpriteRenderer>();

        reload = true;
		setCooldown = cooldown;
		if (GetComponentInParent<PlayerController> ().Player == Controller.Player1) 
		{
			controllerCode = controllerOne;
			fireBomb = shockwaveBomb;
			flipValue = 1;
		} else 
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
		GetDirection ();
		Fire (direction, fireBomb);

        if(animationPlaying)
        {          
            if(throwArm.GetComponent<Animator>().GetBool(BasicAnimator.animAttack) == false)
            {
                // ITS DONE
                animationPlaying = false;
                throwArmSprite.enabled = false;
                armSprite.enabled = true;
            } 
        }		
	}


	void Fire(Vector2 Direction, GameObject fireObj)
	{
		if (((Mathf.Abs(Direction.x) > sensitivity && reload) || (Mathf.Abs(Direction.y) > sensitivity && reload) || Input.GetKeyDown(KeyCode.P)) && clone == null) 
		{	
			if (Input.GetKeyDown (KeyCode.P)) 
			{
				Direction = new Vector2 (1, 1);	
			}
			if (Direction.x < 0) 
			{
				transform.parent.localScale = new Vector3 (-1, transform.parent.localScale.y, transform.parent.localScale.z);
			} else 
			{
				transform.parent.localScale = new Vector3 (1, transform.parent.localScale.y, transform.parent.localScale.z);
			}


			clone = (GameObject)Instantiate (fireObj, transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Rigidbody2D> ().AddForce (Direction.normalized * speed);
			clone.GetComponent<Rigidbody2D> ().gravityScale = flipValue;
			reload = false;
			setCooldown = cooldown;
			direction = Vector2.zero;



            AnimationAttacking(Direction.normalized);
      
		}
		Reload ();
	}

    void AnimationAttacking(Vector2 dir)
    {
        throwArmSprite.enabled = true;
        armSprite.enabled = false;
        Quaternion rot = Quaternion.LookRotation(dir);
        throwArm.transform.rotation = new Quaternion(0, 0, rot.z, rot.w);
        throwArmAnim.Attack();
        animationPlaying = true;

    }

    void GetDirection()
	{
		float x = Input.GetAxisRaw (rightXAxis);
		float y = Input.GetAxisRaw (rightYAxis);

		direction = new Vector2(x, y);

	}


	void Reload()
	{
		setCooldown -= Time.deltaTime;
		if (setCooldown < 0 && !reload) 
		{
			reload = true;
		}
	}

}
