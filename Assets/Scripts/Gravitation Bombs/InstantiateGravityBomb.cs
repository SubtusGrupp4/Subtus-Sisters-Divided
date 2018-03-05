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
    private Vector2 lookingAt;

    private bool isFlipped = false;


    void Start()
    {
        armAnim = arm.GetComponent<BasicAnimator>();
        throwArmAnim = throwArm.GetComponent<BasicAnimator>();

        armSprite = arm.GetComponent<SpriteRenderer>();
        throwArmSprite = throwArm.GetComponent<SpriteRenderer>();

        reloaded = true;
        setCooldown = cooldown;

        if (playerController.player == Controller.Player1)
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
        lookingAt = transform.parent.transform.parent.localScale;

        GetDirection();

        if(!playerController.crawling)
        Fire(direction, fireBomb);

        if (animationPlaying)
        {
            if (throwArm.GetComponent<Animator>().GetBool(BasicAnimator.animAttack) == false)
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
            if (Input.GetKeyDown(KeyCode.P))
                Direction = new Vector2(1, 1);

            //Moonwalking


            /*if (Direction.x < 0) 
			{
				transform.parent.localScale = new Vector3 (-1 * transform.parent.transform.parent.localScale.x, transform.parent.localScale.y, transform.parent.localScale.z);
			} else 
			{
				transform.parent.localScale = new Vector3 (1 * transform.parent.transform.parent.localScale.x, transform.parent.localScale.y, transform.parent.localScale.z);
			}
            */


            AnimationAttacking(Direction);

            clone = Instantiate(fireObj, transform.position, Quaternion.identity);
            clone.GetComponent<Rigidbody2D>().AddForce(Direction.normalized * speed);
            clone.GetComponent<Rigidbody2D>().gravityScale = flipValue;
            reloaded = false;
            setCooldown = cooldown;
            direction = Vector2.zero;
        }

        Reload();
    }

    void AnimationAttacking(Vector2 dir)
    {
        float dirXToOne = dir.x > 0 ? 1 : -1;
        float LookAtToOne = lookingAt.x > 0 ? 1 : -1;

        // In short, we make sure the arm always throw "forward" from a sprite sense, and then when we want to cast forward, we just flip it aswell. 
        // It gets the most natrual look.

        // Depending on which direction we're looking, we decide which direction is "forward".
        if (lookingAt.x > 0)
            dir = new Vector2(Mathf.Abs(dir.x), dir.y);
        else
            dir = new Vector2(Mathf.Abs(dir.x) *-1, (dir.y) );

        throwArmSprite.enabled = true;
        armSprite.enabled = false;

        Quaternion rot = Quaternion.LookRotation(dir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        throwArm.transform.rotation = new Quaternion(0, 0, rot.z, rot.w);

        // Animation for the arm.
        throwArmAnim.Attack();

        if ( dirXToOne != LookAtToOne && !isFlipped)
        {
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
        if (clone == null && !reloaded && setCooldown > 0f)
            setCooldown -= Time.deltaTime;

        if (setCooldown <= 0f && !reloaded)
        {
            setCooldown = cooldown;
            reloaded = true;
        }
    }
}
