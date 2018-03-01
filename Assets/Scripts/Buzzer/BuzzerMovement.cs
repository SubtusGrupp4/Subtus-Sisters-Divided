using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

// TODO: Change so that the player target is always GameManager.instance.playerBot
// TODO: Create a target on the start position if there is none

[RequireComponent(typeof(Rigidbody2D))]
public class BuzzerMovement:MonoBehaviour 
{

    [Header("Target")]
    [SerializeField]
    private BuzzerTarget buzzerTarget; // The target GameObject that the buzzer tries to fly around
    private Transform playerTarget; // The player the buzzer will go after. Should be changed later to always be PlayerBot
    private float followDistance; 

    [Header("Movement")]
    [SerializeField]
    private float speed = 4f; 

    // Min and max for the amount of time between changing directions
    [SerializeField]
    private float moveTimeMin = 0.2f; 
    [SerializeField]
    private float moveTimeMax = 0.5f; 

    private Vector2 flyDir; // The direction in which the buzzer is trying to move
    private bool changePath = true; // When true, the buzzer will change it's flyDir

    private Vector2 targetPos;          // The position of the target

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Attacking")]
    [SerializeField]
    private float attackTimeWait = 2f;
    [SerializeField]
    private float attackSpeed = 5f;
    [SerializeField]
    private float attackDistance = 5f;

    private bool attacking = false;
    private float attackTime;

    private BasicAnimator bodyAnim;

    
    [Header("Audio")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string idleEvent;
    [FMODUnity.EventRef]
    [SerializeField]
    private string attackEvent;
    [FMODUnity.EventRef]
    [SerializeField]
    private string flyingEvent;

    [SerializeField]
    private FMODUnity.StudioEventEmitter emitter;
    /*
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private AudioClip idleSound;
    [SerializeField]
    private AudioClip flyingSound;
    private AudioSource audioSource;
    */

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // If the BasicAnimator component is missing, add it. Should not need any configuration
        if(GetComponent<BasicAnimator>() == null)
            gameObject.AddComponent<BasicAnimator>();

        bodyAnim = GetComponent<BasicAnimator>();
        targetPos = buzzerTarget.transform.position;

        playerTarget = GameManager.instance.playerBot;
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.Event = idleEvent;
        emitter.Play();
    }

    void Update ()
    {
        followDistance = buzzerTarget.radius;

        // If there is no target, stop the buzzer from moving at all
        if (buzzerTarget == null)
        {
            Debug.LogError("The buzzer needs a BuzzerTarget");
            return;
        }

        // TODO: Does this new solution work? Check Git for the old version (2018-02-09)
        if (attacking)      // If attacking
            sr.flipX = rb.velocity.x < 0f ? true : false;           // Set the sprite being flipped in the direction of travel
        else                // If not attacking
        {
            //if(emitter.Event != idleEvent)
               //emitter.Event = idleEvent;

            float targetDistance = Vector2.Distance(transform.position, targetPos);     // How far away the buzzer is from the target

            if (targetDistance < followDistance)    // If inside the target radius
            {
                if (changePath)                     // If changepath, give a random direction
                {
                    float moveTime = Random.Range(moveTimeMin, moveTimeMax);                // Randomize the moveTime
                    flyDir = new Vector2(Random.value * 2f - 1f, Random.value * 2f - 1f);   // Randomize the direction
                    changePath = false;                                                     // Prevent the direction being changed again
                    StartCoroutine(MoveInterval(moveTime));                                 // Change the path again after moveTime amount of time
                }

                flyDir = Vector2.ClampMagnitude(flyDir, 3f);    // Clamp the flying speed
                if (flyDir != Vector2.zero)
                    rb.AddForce(flyDir * speed);                // Fly in the direction flyDir TODO: Base this on Time.deltaTime?
            }
            else    // If outside the target radius
            {
                flyDir = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);   // Set the direction towards the target
                flyDir = flyDir * (targetDistance - followDistance);    // Higher speed the farther outside the radius the buzzer is. Not necessary since clamped?
                flyDir = Vector2.ClampMagnitude(flyDir, 3f);            // Clamp the speed when outside of the circle
                rb.AddForce(flyDir);                                    // Fly towards the target
                changePath = true;                                      // Set to true so that the path will be changed immediately when back in the target radius
            }
            sr.flipX = flyDir.x < 0f ? true : false;                // Set the sprite being flipped in the direction the buzzer is trying to fly towards

            float playerDistance = Vector2.Distance(transform.position, playerTarget.position);     // Get the distance to the player

            if (playerDistance < attackDistance && playerTarget.GetComponent<PlayerController>().isActive)  // If the player is closer than the attack distance
                StartCoroutine(AttackInterval(attackTimeWait));                                             // Start the attacking sequence
        }

        // Raycast in the direction of travel
        Vector2 rayOrigin = transform.position + new Vector3(rb.velocity.normalized.x, rb.velocity.normalized.y);

        Vector2 rayDirection = rb.velocity;
        rayDirection.Normalize();

        float rayDistance = 2f;

        LayerMask layer = 1 << 0;   // TODO: Set this correctly

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);
        Color color = hit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, rayDirection, color);

        // If the raycast hits anything but a player, slow down by moving in the opposite direction
        if (hit.transform != null && hit.transform.tag != "Player" && !hit.collider.isTrigger)
            rb.AddForce(-rayDirection, ForceMode2D.Impulse);
    }

    // Changes the direction after a set time
    private IEnumerator MoveInterval(float time)
    {
        yield return new WaitForSeconds(time);
        changePath = true;
    }

    // A sequence of events that composes the attacking
    private IEnumerator AttackInterval(float time)
    {
        bodyAnim.Attack();      // Start the attacking animation
        //emitter.Event = flyingEvent;
        attacking = true;       // Prevent the movement code from running, and another attack from triggering
        rb.AddForce(new Vector2(0f, -speed * 24f), ForceMode2D.Impulse);    // Move upwards to show the imminent attack
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 2f);              // Clamp the speed
        Vector2 targetingPos = playerTarget.position;                       // Target where the player currently is, but only once
        sr.flipX = rb.velocity.x < 0f ? true : false;                       // Flip the sprite in the direction of travel

        yield return new WaitForSeconds(time);                              // Wait

        //emitter.Event = attackEvent;
        //emitter.Play();
        Vector2 attackDir = new Vector2(targetingPos.x - transform.position.x, targetingPos.y - transform.position.y);  // Get the direction towards where the player was
        rb.AddForce(attackDir.normalized * attackSpeed, ForceMode2D.Impulse);   // Fly quickly towards where the player used to be
        sr.flipX = attackDir.x < 0f ? true : false;     // Flip the sprite in the direction of attack

        yield return new WaitForSeconds(time);  // Wait

        attacking = false;      // Stop attackin, re-enabling the movement code
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collide with player, kill the player
        if (collision.transform.tag == "Player")
            collision.transform.GetComponent<PlayerController>().Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If collide with portal, die
        if (collision.tag == "Portal")
            Destroy(gameObject);
    }
}
