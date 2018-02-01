using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Controller
{
    Player1, Player2
}

public enum AirControll
{
    full, Semi, SemiFull, None
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
    private string controllerCode;

    private string controllerOne = "_C1";
    private string controllerTwo = "_C2";

    private string HorAx = "Horizontal";
    private string VerAx = "Vertical";
    private string jumpInput = "Jump";
    private float distanceGraceForJump = 0.4f; // how faar outside the boxcollider do you want the ray to travel when reseting jump?

    // Components
    private AudioSource myAudio; // Audio source on player object or sound master or game master ??? who knows , we do later.
    private CapsuleCollider2D myBox;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    // Settings
    [HideInInspector]
    public bool isActive;

    private SpriteRenderer sr;

    [Header("Physics")]

    private bool Flipped;
    private int flippValue = 1; // Value between 1 and -1

    [SerializeField]
    private float Speed;
    [SerializeField]
    private float JumpVelocity;
    [SerializeField]
    private AirControll AirControll;
    [SerializeField]
    [Range(0, 1)]
    private float AirControllPrecentage;

    [GiveTag]
    [SerializeField]
    private string[] ResetJumpOn = new string[] { };

    [Header("Sounds")]

    [SerializeField]
    private AudioClip JumpSound;

    [Header("Reviving")]
    [SerializeField]
    private Vector2 lastSafe;
    [SerializeField]
    private GameObject revivePlacerPrefab;
    private GameObject revivePlacer;

    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        myBox = GetComponent<CapsuleCollider2D>();

        if (Player == Controller.Player1)
        {
            controllerCode = controllerOne;
            Flipped = false;
        }
        else
        {
            controllerCode = controllerTwo;
            Flipped = true;
        }

        HorAx += controllerCode;
        VerAx += controllerCode;
        jumpInput += controllerCode;

        if (Flipped)
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
            rigidbody2D.velocity = Vector2.up * flippValue * JumpVelocity;

            //  GetComponent<BoxCollider2D>().sharedMaterial.friction = 0;

            myAudio.PlayOneShot(JumpSound); // needs change?? need landing sound ??
            // play jump animation
        }

        // Input Manager
        X = Input.GetAxis(HorAx); // Valute between 0 and 1 from input manager.
        float Y = GetComponent<Rigidbody2D>().velocity.y;

        // Round it to nearest .5
        temp = X;
        temp = (float)Math.Round(temp * 2, MidpointRounding.AwayFromZero) / 2;

        if (!inAir)
            temp *= Speed;

        // Fixing all the Jumping and shit
        ControllingAir();

        // Creating SavedVelocity.
        if (!inAir)
        {
            rigidbody2D.velocity = new Vector2(temp, Y);
            savedVelocity = rigidbody2D.velocity;
            changedMade = false;
        }

        rigidbody2D.velocity = new Vector2(temp, Y);
    }

    private void ControllingAir()
    {
        // Full Controll
        if (AirControll == AirControll.full && inAir)
            temp *= Speed * AirControllPrecentage;

        // Semi Controll 
        if (inAir && AirControll != AirControll.full)
        {
            // If it's not Semi it's No controll, in which case it will just use, temp = savedvelocity.
            if (AirControll == AirControll.Semi)
            {
                if (temp > 0) // go right
                {
                    if (savedVelocity.x < 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControllPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * Speed * AirControllPrecentage;
                        changedMade = true;
                    }
                }
                else if (temp < 0)
                {
                    if (savedVelocity.x > 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControllPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * Speed * AirControllPrecentage;
                        changedMade = true;
                    }

                }
            }
            temp = savedVelocity.x;
        }
    }

    public void Die()
    {
        // Mabey needed for Ressurect in future.
        isActive = false;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.isKinematic = true;

        Vector2 spawnPos = new Vector2(lastSafe.x, 0f);
        revivePlacer = Instantiate(revivePlacerPrefab, spawnPos, Quaternion.identity);
        revivePlacer.GetComponent<RevivePlacer>().Initialize(Player, transform);
        sr.enabled = false;

        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingOne, transform);
    }

    public void Ressurect()
    {
        // ^^^^^^
        transform.position = lastSafe;
        isActive = true;
        rigidbody2D.isKinematic = false;
        sr.enabled = true;

        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth);
    }

    private void ResetJump()
    {
        // If we asume we're always falling until told otherwise we get a more proper behaviour when falling off things.
        inAir = true;
        for (int i = 0; i < ResetJumpOn.Length; i++)
        {
            for (int l = -1; l < 2; l += 2)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flippValue), Mathf.Abs((myBox.size.y
                * GetComponent<Transform>().localScale.y)) / 2 + distanceGraceForJump);

                Debug.DrawRay(transform.position, new Vector2(l, -flippValue), Color.red);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == ResetJumpOn[i])
                    {
                        if (Mathf.Abs(objHit[j].normal.x) < wallNormal) // So we cant jump on walls.
                        {
                            inAir = false;
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
            for (int i = 0; i < ResetJumpOn.Length; i++)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flippValue),
                    Mathf.Abs((myBox.size.y * GetComponent<Transform>().localScale.y)) / 2 + distanceGraceForJump);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag != ResetJumpOn[i])
                        continue;

                    if (objHit[j].collider != null && Mathf.Abs(objHit[j].normal.x) > 0.1f && Mathf.Abs(objHit[j].normal.x) < wallNormal && inAir == false)
                    {
                        Debug.Log("hej obhHitNormalX " + objHit[j].normal.x + " L " + l);

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
                            offSet *=  0.5f;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Portal")
        {
            Die();
        }
        else if(collision.transform.tag == "Revive")
        {
            Destroy(collision.gameObject);
            Ressurect();
        }
    }
}
