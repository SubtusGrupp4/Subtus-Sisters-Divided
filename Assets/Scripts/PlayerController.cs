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
    private bool inAir;
    float temp;
    private float wallNormal = 0.9f;

    // Input Manager 
    public Controller Player;
    [NonSerialized]
    public string controllerCode;

  
    public const string controllerOne = "_C1";
    public const string controllerTwo = "_C2";

    private string horAx = "Horizontal";
    private string verAx = "Vertical";
    private string jumpInput = "Jump";
    private float distanceGraceForJump = 0.4f; // how faar outside the boxcollider do you want the ray to travel when reseting jump?

    // Components
    private AudioSource myAudio; // Audio source on player object or sound master or game master ??? who knows , we do later.
    private CapsuleCollider2D myBox;
    private float rayOffSetX;
    private float rayOffSetY;
    private BasicAnimator bodyAnim;
    private BasicAnimator armAnim;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    // Settings
    [HideInInspector]
    public bool isActive;

    private SpriteRenderer sr;

    [Header("Physics")]

    private bool flipped;
    private int flippValue = 1; // Value between 1 and -1

    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpVelocity;
    [SerializeField]
    private AirControl AirControl;
    [SerializeField]
    [Range(0, 1)]
    private float AirControlPrecentage;

    [GiveTag]
    [SerializeField]
    private string[] resetJumpOn = new string[] { };

    [Header("Sounds")]

    [SerializeField]
    private AudioClip jumpSound;

    [Header("Reviving")]
    [SerializeField]
    private Vector2 lastSafe;
    [SerializeField]
    private GameObject revivePlacerPrefab;
    private GameObject revivePlacer;

    void Awake()
    {
        rayOffSetX = GetComponent<CapsuleCollider2D>().size.x * transform.localScale.x / 2;
        rayOffSetY = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y / 2;

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

        myAudio = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        myBox = GetComponent<CapsuleCollider2D>();

        if (Player == Controller.Player1)
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

        if (flipped)
            Flip();

        isActive = true;
        inAir = true;

        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isActive)
        {
            ResetJump();
        }
    }
    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            NormalizeSlope();

            if (!inAir)
                lastSafe = transform.position;
        }
    }

    private void Move()
    {
        // Temporary
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Flip();
        }
        // JUMP
        if (Input.GetAxis(jumpInput) > 0 && (!inAir))
        {
            inAir = true;
            rigidbody2D.velocity = Vector2.up * flippValue * jumpVelocity;

            //  GetComponent<BoxCollider2D>().sharedMaterial.friction = 0;

            myAudio.PlayOneShot(jumpSound); // needs change?? need landing sound ??
                                            // play jump animation
        }

        // Input Manager
        X = Input.GetAxis(horAx); // Valute between 0 and 1 from input manager.
        float Y = GetComponent<Rigidbody2D>().velocity.y;

        // Round it to nearest .5
        temp = X;
        temp = (float)Math.Round(temp * 2, MidpointRounding.AwayFromZero) / 2;

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
        if (AirControl == AirControl.Full && inAir)
            temp *= speed * AirControlPrecentage;

        // Semi Controll 
        if (inAir && AirControl != AirControl.Full)
        {
            // If it's not Semi it's No controll, in which case it will just use, temp = savedvelocity.
            if (AirControl == AirControl.Semi)
            {
                if (temp > 0) // go right
                {
                    if (savedVelocity.x < 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * AirControlPrecentage;
                        changedMade = true;
                    }
                }
                else if (temp < 0)
                {
                    if (savedVelocity.x > 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * AirControlPrecentage;
                        changedMade = true;
                    }

                }
            }
            temp = savedVelocity.x;
        }
    }

    public void Die()
    {
        if (isActive)
        {
            // Mabey needed for Ressurect in future.
            isActive = false;
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.isKinematic = true;

            Vector2 spawnPos = new Vector2(lastSafe.x, 0f);
            revivePlacer = Instantiate(revivePlacerPrefab, spawnPos, Quaternion.identity);
            revivePlacer.GetComponent<RevivePlacer>().Initialize(Player, transform);

            if (Player == Controller.Player1)
                GameManager.instance.playerTop = null;
            else
                GameManager.instance.playerBot = null;

            Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingOne, transform);

            Destroy(gameObject);
        }
    }

    private void ResetJump()
    {
        // If we asume we're always falling until told otherwise we get a more proper behaviour when falling off things.
        inAir = true;

        // transform.parent = null;

        //
        //
        bodyAnim.Falling(true);
        armAnim.Falling(true);
        //
        //
        for (int i = 0; i < resetJumpOn.Length; i++)
        {
            for (int l = -1; l < 2; l += 2)
            {
                /*
                (transform.position + new Vector3((rayOffSetX - 0.05f) * l, 0, 0),
                -Vector2.up * flipValue,
                rayOffSetY + distanceGraceForFalling); */

                objHit = Physics2D.RaycastAll(transform.position + new Vector3((rayOffSetX - 0.05f) * l, 0, 0),
                    -Vector2.up * flippValue,
                rayOffSetY + distanceGraceForJump);

                Debug.DrawRay(transform.position + new Vector3((rayOffSetX - 0.05f) * l, 0, 0),
                    -Vector2.up * flippValue,
                    Color.red);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == resetJumpOn[i])
                    {
                        if (Mathf.Abs(objHit[j].normal.x) < wallNormal) // So we cant jump on walls.
                        {
                            /*
                            if (resetJumpOn[i] == "MovingFloor" || objHit[j].transform.GetComponent<MovingPlatform>() != null)
                                transform.parent = objHit[j].transform;
*/
                            inAir = false;


                            //
                            //
                            bodyAnim.Falling(false);
                            armAnim.Falling(false);
                            //
                            //

                            break;
                        }
                    }
                }
            }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Portal")
        {
            Die();
        }
        else if (collision.transform.tag == "Revive")
        {
            GameObject resPlayer;
            if (Player == Controller.Player1)
            {
                resPlayer = Instantiate(GameManager.instance.playerBotPrefab, SafepointManager.instance.botCheckpoint.position, Quaternion.identity);
                GameManager.instance.playerBot = resPlayer.transform;
            }
            else
            {
                resPlayer = Instantiate(GameManager.instance.playerTopPrefab, SafepointManager.instance.topCheckpoint.position, Quaternion.identity);
                GameManager.instance.playerTop = resPlayer.transform;
            }

            Destroy(collision.gameObject);
            Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth);
        }
    }
}
