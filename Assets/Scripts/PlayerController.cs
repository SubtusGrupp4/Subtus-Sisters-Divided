using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Controller
{
    Player1, Player2
}

public class PlayerController : MonoBehaviour
{

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
    
    private AudioSource myAudio; // Audio source on player object or sound master or game master ??? who knows , we do later.
    private BoxCollider2D myBox;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    private bool inAir;
    [HideInInspector]
    public bool isActive;

    [Header("Physics")]

    public bool flipped;
    private int flippValue = 1; // Value between 1 and -1

    public float WalkingSpeed;
    public float Speed; 
    public float JumpVelocity;

    [GiveTag]
    public string[] ResetJumpOn = new string[] { };

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

        if (Input.GetAxis(jumpInput) > 0 && (!inAir))
        {
            inAir = true;
            rigidbody2D.velocity = Vector2.up * flippValue * JumpVelocity;

            myAudio.PlayOneShot(jumpSound); // needs change?? need landing sound ??
            // play jump animation
        }

        
        float X = Input.GetAxis(HorAx); // Valute between 0 and 1 from input manager.

        if (X <= stillGrace)
            X = 0;
        else if (X > stillGrace && X <= walkingCutOff)
            X = WalkingSpeed;
        else if (X > walkingCutOff)
            X = Speed;

        float Y = GetComponent<Rigidbody2D>().velocity.y;

        rigidbody2D.velocity = new Vector2(X, Y);
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
