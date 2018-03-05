using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Controller
{
    Player1, Player2
}

public enum AirControl
{
    Full, Semi, SemiFull, None
}

public class PlayerController : MonoBehaviour
{
    // Saved State for Different Jumping states.
    private float X;
    private Vector2 savedVelocity;
    private bool changedMade;
    [HideInInspector]
    public bool inAir;
    private float timeUntilAirborn = 0.1f;
    private float airbornTimer;
    private bool jumpAxisInUse;
    float temp;
    private float wallNormal = 0.9f;

    // Input Manager 
    public Controller player;
    [NonSerialized]
    public string controllerCode;

    public const string controllerOne = "_C1";
    public const string controllerTwo = "_C2";

    private string horAx = "Horizontal";
    private string verAx = "Vertical";
    private string jumpInput = "Jump";
    private string crawlInput = "CrawlInput";
    private float distanceGraceForJump = 0.02f; // how faar outside the boxcollider do you want the ray to travel when reseting jump?

    // Components
    private CapsuleCollider2D myBox;
    private float capsuleRadiusX;
    private float capsuleRadiusY;
    private float capsuleOffSetX;
    private float capsuleOffSetY;

    public BasicAnimator bodyAnim;
    public BasicAnimator armAnim;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    // Settings
    [HideInInspector]
    public bool isActive;

    bool landing;
    [HideInInspector]
    public bool crawling;
    [HideInInspector]
    public bool pulling;

    private SpriteRenderer sr;

    [Header("Physics")]

    private bool flipped;
    private int flippValue = 1; // Value between 1 and -1

    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpVelocity;
    [SerializeField]
    private AirControl airControl;
    [SerializeField]
    [Range(0, 1)]
    private float airControlPrecentage;

    [GiveTag]
    [SerializeField]
    private string[] resetJumpOn = new string[] { };

    // FMOD Audio
    private MovementAudio movementAudio;

    [Header("Crawling")]
    private bool insideCrawling = false;
    private bool crawlAxisInUse = false;

    public float crawlSpeed;
    private float savedSpeed;

    public Vector2 crawlColliderOffset;
    public Vector2 crawlColliderSize;

    private Vector2 savedColliderOffSet;
    private Vector2 savedColliderSize;

    [HideInInspector]
    public Vector2 lastSafe;
    private Reviving reviveScript;

    void Awake()
    {
        myBox = GetComponent<CapsuleCollider2D>();

        capsuleRadiusX = myBox.size.x * transform.localScale.x / 2;
        capsuleRadiusY = myBox.size.y * transform.localScale.y / 2;

        capsuleOffSetX = myBox.offset.x;
        capsuleOffSetY = myBox.offset.y;

        savedColliderSize = myBox.size;
        savedColliderOffSet = myBox.offset;

        savedSpeed = speed;

        // FIX JUMP
        //
        // Arm, ThrowArm, Hide sprite renderer ocn "Arm" when attacking, show "ThrowArm" wait until throw is done, then hide throw arm
        // and show normal "arm" again.
        //
        bodyAnim = GetComponent<BasicAnimator>();
        GameObject arm = this.transform.Find("Arm").gameObject;
        armAnim = arm.GetComponent<BasicAnimator>();
        //
        //

        rigidbody2D = GetComponent<Rigidbody2D>();


        if (player == Controller.Player1)
        {
            controllerCode = controllerOne;
            flipped = false;
        }
        else
        {
            controllerCode = controllerTwo;
            flipped = true;
        }

        horAx += controllerCode;
        verAx += controllerCode;
        jumpInput += controllerCode;
        crawlInput += controllerCode;

        if (flipped)
            Flip();

        isActive = true;
        inAir = true;

        sr = GetComponent<SpriteRenderer>();
        movementAudio = GetComponent<MovementAudio>();
        reviveScript = GetComponent<Reviving>();
    }

    void Update()
    {
        if (isActive)
            ResetJump();

        if (bodyAnim.GetLandState() == false)
            landing = false;

        crawling = bodyAnim.GetCrawlState();

        if (player == Controller.Player1)
            ToggleCrawl();

    }
    private void FixedUpdate()
    {
        if (isActive)
        {

            Move();

            NormalizeSlope();
        }
    }

    void ToggleCrawl()
    {
        if (crawling)
        {
            // Collider
            myBox.size = crawlColliderSize;
            myBox.offset = crawlColliderOffset;
            myBox.direction = CapsuleDirection2D.Horizontal;

            speed = crawlSpeed;
        }
        else
        {
            // Collider
            myBox.size = savedColliderSize;
            myBox.offset = savedColliderOffSet;
            myBox.direction = CapsuleDirection2D.Vertical;

            speed = savedSpeed;
        }

        if (Input.GetAxisRaw(crawlInput) > 0 && !crawlAxisInUse)
        {
            Collider2D[] allObjs;
            bool blocked = false;
            crawlAxisInUse = true;

            if (!crawling)
                allObjs = Physics2D.OverlapBoxAll((Vector2)transform.position + crawlColliderOffset + new Vector2(0, 0.01f), crawlColliderSize, 0);
            else
                allObjs = Physics2D.OverlapBoxAll((Vector2)transform.position + savedColliderOffSet + new Vector2(0, 0.01f), savedColliderSize, 0);
            /*
            // We want to difference between transiton to crawl, and is currently crawling.
            if (!crawling)
                allObjs = Physics2D.CapsuleCastAll((Vector2)transform.position + crawlColliderOffset ,
                    crawlColliderSize,
                    CapsuleDirection2D.Horizontal,
                    0,
                    Vector2.right);
            else
                allObjs = Physics2D.CapsuleCastAll((Vector2)transform.position + savedColliderOffSet ,
                    savedColliderSize,
                    CapsuleDirection2D.Vertical,
                    0,
                    Vector2.right);
                    */


            for (int i = 0; i < allObjs.Length; i++)
            {
                if (blocked)
                    break;
                for (int j = 0; j < resetJumpOn.Length; j++)
                {
                    if (blocked)
                        break;
                    if (allObjs[i].transform.tag == resetJumpOn[j])
                    {
                        blocked = true;
                        break;
                    }
                }
            }
            Debug.Log("blocked" + blocked);
            if (!blocked)
            {
                insideCrawling ^= true;
                //


                bodyAnim.Crawl(insideCrawling);
                armAnim.Crawl(insideCrawling);
            }

        }
        else if (Input.GetAxisRaw(crawlInput) == 0)
        {
            // axisInUse is used to make sure that you dont trigger it alot when you hold down the button.
            crawlAxisInUse = false;
        }
    }

    private void Jump()
    {
        inAir = true;
        rigidbody2D.velocity = Vector2.up * flippValue * jumpVelocity;

        bodyAnim.Jump();
        armAnim.Jump();

        movementAudio.Jump();
    }
    private void Move()
    {
        // JUMP
        if (Input.GetAxis(jumpInput) > 0 && !jumpAxisInUse)
        {
            jumpAxisInUse = true;
            if (!inAir && !landing && !crawling && !pulling)
            {
                Jump();              
            }
        }
        else if(Input.GetAxis(jumpInput) == 0)
            jumpAxisInUse = false;

        // Input Manager
        X = Input.GetAxis(horAx); // Valute between 0 and 1 from input manager.
        float Y = GetComponent<Rigidbody2D>().velocity.y;

        // Round it to nearest .5
        temp = X;
        temp = (float)Math.Round(temp * 2, MidpointRounding.AwayFromZero) / 2;

        if (Mathf.Abs(temp) <= 0.5f)
        {
            bodyAnim.ToggleWalk(true);
            armAnim.ToggleWalk(true);
        }
        else
        {
            bodyAnim.ToggleWalk(false);
            armAnim.ToggleWalk(false);
        }

        if (!inAir)
            temp *= speed;

        // Fixing all the Jumping and shit
        ControllingAir();

        // Creating SavedVelocity.
        if (!inAir)
        {
            rigidbody2D.velocity = new Vector2(temp, Y);
            savedVelocity = rigidbody2D.velocity;
            changedMade = false;
        }

        //
        //
        bodyAnim.Walking(new Vector2(temp, Y), true);
        armAnim.Walking(new Vector2(temp, Y), false);
        //
        //

        rigidbody2D.velocity = new Vector2(temp, Y);
    }

    private void ControllingAir()
    {
        // Full Controll
        if (airControl == AirControl.Full && inAir)
            temp *= speed * airControlPrecentage;

        // Semi Controll 
        if (inAir && airControl != AirControl.Full)
        {
            // If it's not Semi it's No controll, in which case it will just use, temp = savedvelocity.
            if (airControl == AirControl.Semi)
            {
                if (temp > 0) // go right
                {
                    if (savedVelocity.x < 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * airControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * airControlPrecentage;
                        changedMade = true;
                    }
                }
                else if (temp < 0)
                {
                    if (savedVelocity.x > 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * airControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * airControlPrecentage;
                        changedMade = true;
                    }

                }
            }
            temp = savedVelocity.x;
        }
    }

    private void ResetJump()
    {
        // If we asume we're always falling until told otherwise we get a more proper behaviour when falling off things.


        airbornTimer += Time.deltaTime;

        // transform.parent = null;
        bool tempInAir = false;
        if (airbornTimer > timeUntilAirborn)
        {
            tempInAir = true;
            //
            //
            bodyAnim.Falling(true);
            armAnim.Falling(true);
            //
            //
        }

        for (int i = 0; i < resetJumpOn.Length; i++)
        {
            bool quickBreak = false;

            for (int l = -1; l < 2; l += 2)
            {
                if (quickBreak)
                    break;

                /*
                (transform.position + new Vector3((rayOffSetX - 0.05f) * l, 0, 0),
                -Vector2.up * flipValue,
                rayOffSetY + distanceGraceForFalling); */

                objHit = Physics2D.RaycastAll(transform.position + new Vector3((capsuleRadiusX - 0.05f) * l, 0, 0) + new Vector3(capsuleOffSetX * transform.localScale.x, capsuleOffSetY * flippValue, 0),
                    -Vector2.up * flippValue,
                capsuleRadiusY + distanceGraceForJump);

                Debug.DrawRay(transform.position + new Vector3((capsuleRadiusX - 0.05f) * l, 0, 0) + new Vector3(capsuleOffSetX * transform.localScale.x, capsuleOffSetY * flippValue, 0),
                    -Vector2.up * flippValue,
                    Color.red);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == resetJumpOn[i])
                    {
                        if (Mathf.Abs(objHit[j].normal.x) < wallNormal) // So we cant jump on walls.
                        {

                            if (inAir)
                            {
                                // LANDING
                                //play land sound
                                if (bodyAnim.GetJumpState() == false)
                                {
                                    Debug.Log("LANDING");

                                    bodyAnim.Land();
                                    armAnim.Land();

                                    movementAudio.Landing();
                                    landing = true;
                                }

                                // play land animation

                            }

                            /*
                            if (resetJumpOn[i] == "MovingFloor" || objHit[j].transform.GetComponent<MovingPlatform>() != null)
                                transform.parent = objHit[j].transform;
*/
                            // inAir = false;
                            tempInAir = false;
                            lastSafe = transform.position;
                            airbornTimer = 0;

                            //
                            //
                            bodyAnim.Falling(false);
                            armAnim.Falling(false);
                            //
                            //

                            // Send what type of ground the player is standig on
                            movementAudio.SetGroundType(objHit[j].transform.gameObject.layer);

                            quickBreak = true;
                        }
                    }
                }
            }
            inAir = tempInAir;
        }
    }

    public void Flip()
    {
        // mabey flip sprite insted??
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.x);
        rigidbody2D.gravityScale *= -1; // E.V move this to portal ??
        flippValue *= -1;
    }

    void NormalizeSlope()
    {
        float slopeFriction = 0.11f;
        // Small optimization (if first ray hit we dont raycast a second time).
        bool slope = false;

        // Raycasting twice, meanwhile using L as direction because we want L to be between -1 and 1 so we raycast left and right...
        for (int l = -1; l < 2; l += 2)
        {
            // Attempt vertical normalization
            for (int i = 0; i < resetJumpOn.Length; i++)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flippValue),
                    Mathf.Abs((myBox.size.y * GetComponent<Transform>().localScale.y)) / 2 + distanceGraceForJump);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag != resetJumpOn[i])
                        continue;

                    if (objHit[j].collider != null && Mathf.Abs(objHit[j].normal.x) > 0.1f && Mathf.Abs(objHit[j].normal.x) < wallNormal && inAir == false)
                    {


                        slopeFriction *= Mathf.Abs(objHit[j].normal.x);

                        Rigidbody2D body = GetComponent<Rigidbody2D>();
                        // Apply the opposite force against the slope force 
                        body.velocity = new Vector2(body.velocity.x - (objHit[j].normal.x * slopeFriction), body.velocity.y);

                        //Move Player up or down to compensate for the slope below them
                        Vector3 pos = transform.position;
                        float offSet = 0;
                        //              "-1"          normalen     *       hastigheten          *       deltatime     *  hastigheten - (1 / -1)
                        offSet += (flippValue * -1) * objHit[j].normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - objHit[j].normal.x > 0 ? 1f : -1f);

                        if (offSet * flippValue > 0)
                            offSet *= 0.5f;
                        else
                            offSet *= 2;

                        pos.y += offSet;
                        transform.position = pos;

                        inAir = false;
                        slope = true;

                    }
                }
            }
            if (slope)
                break;
        }
    }
}
