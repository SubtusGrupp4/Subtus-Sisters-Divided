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
    full, Semi, None
}

public class PlayerController : MonoBehaviour
{

    private float X;
    private Vector2 savedVelocity;
    private bool changedMade;

    // KOMMENTAR  KOLlA OM VI KAN FIXA SMÅ HOPP.

    public Controller player;
    private string controllerCode;

    private string controllerOne = "_C1";
    private string controllerTwo = "_C2";

    private string HorAx = "Horizontal";
    private string VerAx = "Vertical";
    private string jumpInput = "Jump";
    private float distanceGraceForJump = 0.1f; // how faar outside the boxcollider do you want the ray to travel when reseting jump?

    // For Input manager, How much on the joystick is needed before you start moving?
    private float stillGrace = 0.1f;
    private float walkingCutOff = 0.4f;
    // REMOVE ??

    private AudioSource myAudio; // Audio source on player object or sound master or game master ??? who knows , we do later.
    private BoxCollider2D myBox;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    private bool inAir;
    [HideInInspector]
    public bool isActive;

    [Header("Physics")]

    [SerializeField]
    private bool flipped;
    private int flippValue = 1; // Value between 1 and -1

    [SerializeField]
    private float Speed;
    [SerializeField]
    private float JumpVelocity;
    [SerializeField]
    private AirControll AirControll;
    [SerializeField]
    [Range(0, 1)]
    private float airControlPrecentage;

    [GiveTag]
    [SerializeField]
    private string[] ResetJumpOn = new string[] { };

    [Header("Sounds")]

    [SerializeField]
    private AudioClip jumpSound;

    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        myBox = GetComponent<BoxCollider2D>();

        if (player == Controller.Player1)
            controllerCode = controllerOne;
        else
            controllerCode = controllerTwo;

        HorAx += controllerCode;
        VerAx += controllerCode;
        jumpInput += controllerCode;

        if (flipped)
            Flip();

        isActive = true;
        inAir = true;
    }

    void Update()
    {
        if (isActive)
        {
            ResetJump();
        }
        // if (inAir)
        //    GetComponent<Rigidbody2D>().sharedMaterial.friction = 0;


    }
    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
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

            myAudio.PlayOneShot(jumpSound); // needs change?? need landing sound ??
            // play jump animation
        }

        X = Input.GetAxis(HorAx); // Valute between 0 and 1 from input manager.
        float Y = GetComponent<Rigidbody2D>().velocity.y;

        float temp = X;
        temp = (float)Math.Round(temp * 2, MidpointRounding.AwayFromZero) / 2;

        if (!inAir)
            temp *= Speed;


        // Full Controll
        if (AirControll == AirControll.full && inAir)
            temp *= Speed * airControlPrecentage;

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
                        savedVelocity.x += savedVelocity.x * airControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * Speed * airControlPrecentage;
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
                    else if(savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp* Speed * airControlPrecentage;
                        changedMade = true;
                    }

                }
            }
            temp = savedVelocity.x;
        }

        // Creating SavedVelocity.
        if (!inAir)
        {
            rigidbody2D.velocity = new Vector2(temp, Y);
            savedVelocity = rigidbody2D.velocity;
            changedMade = false;
        }

        // Applying Speed
        rigidbody2D.velocity = new Vector2(temp, Y);
    }

    public void Die()
    {
        // Mabey needed for Ressurect in future.
        isActive = false;
        rigidbody2D.velocity = Vector2.zero;
    }

    public void Ressurect()
    {
        // ^^^^^^
        isActive = true;
    }

    private void ResetJump()
    {
        inAir = true;
        for (int i = 0; i < ResetJumpOn.Length; i++)
        {
            objHit = Physics2D.RaycastAll(transform.position, -Vector2.up * flippValue, +Mathf.Abs((GetComponent<BoxCollider2D>().size.y
                * GetComponent<Transform>().localScale.y)) / 2 + distanceGraceForJump);

            for (int j = 0; j < objHit.Length; j++)
                if (objHit[j].transform.tag == ResetJumpOn[i])
                {
                    inAir = false;
                    //        GetComponent<BoxCollider2D>().sharedMaterial.friction = 1;
                    break;
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
}
