using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationBomb : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField]
    private int range;
    [SerializeField]
    private int pullForce;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float deActivate;
    [Header("tags from objects to pull")]
    [GiveTag]
    [SerializeField]
    private string[] tagNames;

    [Header("Particles")]
    [SerializeField]
    private GameObject[] particles;
    private List<GameObject> particleClones;

    private GameObject[] ignoreObject;
    private List<GameObject> pullObjects;
    private Rigidbody2D rb;
    private float rotationSpeed;

    private bool buttonPressed;
    private bool targetting;
    private bool gravitationActivated;
    private int decreaseGravity;
    private string activateBomb = "Fire_Bomb_C2";

    [Header("FMOD Audio")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string thrown;
    [SerializeField]
    [FMODUnity.EventRef]
    private string loop;
    [SerializeField]
    [FMODUnity.EventRef]
    private string activate;
    [SerializeField]
    [FMODUnity.EventRef]
    private string deactivate;

    private FMODEmitter emitter;



    void Start()
    {
        emitter = GetComponent<FMODEmitter>();
        particleClones = new List<GameObject>();
        rb = GetComponent<Rigidbody2D>();
        pullObjects = new List<GameObject>();

        ignoreObject = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject IO in ignoreObject)
            Physics2D.IgnoreCollision(IO.GetComponent<BoxCollider2D>(), GetComponent<CircleCollider2D>(), true);

        buttonPressed = true;
        gravitationActivated = false;
        targetting = false;
        decreaseGravity = 0;
        rotationSpeed = 360f;

        FMODUnity.RuntimeManager.PlayOneShot(thrown, transform.position);

        if (transform.position.y > 0f)
            rb.gravityScale = Mathf.Abs(rb.gravityScale);
        else
            rb.gravityScale = -Mathf.Abs(rb.gravityScale);
    }

    void Update()
    {

        ActivateGravitationBomb();
        TargetInRange();
        TargetOutsideRange();
        DestroyAfterDuration();
    }

    void FixedUpdate()
    {
        GravitationPull();
    }

    void DestroyAfterDuration()
    {
        if (!buttonPressed)
        {
            duration -= Time.deltaTime;
            if (duration < 0)
            {
                ResetGravity(0);
                emitter.Stop();
                Destroy(gameObject);
            }
        }
    }

    void ActivateGravitationBomb()
    {

        deActivate -= Time.deltaTime;
        if ((Input.GetAxis(activateBomb) != 0f || Input.GetKeyDown(KeyCode.T)) && buttonPressed)
        {

            foreach (GameObject particle in particles)
            {
                GameObject clone = (Instantiate(particle, transform.position, Quaternion.identity));
                particleClones.Add(clone);
            }

            buttonPressed = false;
            targetting = true;
            gravitationActivated = true;
            rb.bodyType = RigidbodyType2D.Static;

            FMODUnity.RuntimeManager.PlayOneShot(activate, transform.position);
            emitter.SetEvent(loop);
            emitter.Play();
            GetComponent<AudioOnCollision>().isActive = false;
        }
        if ((Input.GetAxis(activateBomb) == 0f || Input.GetKeyUp(KeyCode.T)) && !buttonPressed)
        {
            ResetGravity(0);
            emitter.Stop();
            FMODUnity.RuntimeManager.PlayOneShot(deactivate, transform.position);
            Destroy(gameObject);
        }
        if (deActivate < 0)
        {
            emitter.Stop();
            FMODUnity.RuntimeManager.PlayOneShot(deactivate, transform.position);
            Destroy(gameObject);
        }
    }

    void TargetInRange()
    {
        if (targetting)
        {
            pullObjects.Clear();

            foreach (string tag in tagNames)
            {
                GameObject[] o = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject targetObjects in o)
                {
                    if (transform.position.y < 0f && targetObjects.transform.position.y < 0f || transform.position.y > 0f && targetObjects.transform.position.y > 0f)
                    {
                        float inRange = Vector3.Distance(transform.position, targetObjects.transform.position);
                        if (range > inRange)
                        {
                            pullObjects.Add(targetObjects);
                        }
                    }
                }
            }
            targetting = false;
            StartCoroutine(TargetingTimer());
        }
    }

    private IEnumerator TargetingTimer()
    {
        yield return new WaitForSeconds(0.2f);
        targetting = true;
    }

    void TargetOutsideRange()
    {

        for (int i = 0; i < pullObjects.Count; i++)
        {
            float inRange = Vector3.Distance(transform.position, pullObjects[i].transform.position);
            if (range < inRange)
            {
                pullObjects.Remove(pullObjects[i]);
            }
        }
    }

    void GravitationPull()
    {
        if (gravitationActivated)
        {
            foreach (GameObject toPull in pullObjects)
            {
                toPull.GetComponent<Rigidbody2D>().MoveRotation(toPull.GetComponent<Rigidbody2D>().rotation + rotationSpeed * Time.fixedDeltaTime);

                float distanceSquared = Mathf.Sqrt(Vector3.Distance(transform.position, toPull.transform.position));
                float force = pullForce * (rb.mass * toPull.GetComponent<Rigidbody2D>().mass) / distanceSquared;

                Vector2 distance = transform.position - toPull.transform.position;
                Vector2 forceDirection = (transform.position - toPull.transform.position).normalized;
                Vector2 forceVector = forceDirection * force;
                if (distance.y < 1 && distance.x < 1)
                {
                    decreaseGravity++;
                    toPull.GetComponent<Rigidbody2D>().AddForce(forceVector / -decreaseGravity);
                }
                if (toPull.GetComponent<AIMovement>())
                    toPull.GetComponent<AIMovement>().Stun(0.1f);


                toPull.GetComponent<Rigidbody2D>().AddForce(forceVector);
            }
        }
    }

    void ResetGravity(int integer)
    {
        foreach (GameObject toPull in pullObjects)
        {
            toPull.GetComponent<Rigidbody2D>().velocity = new Vector2(toPull.GetComponent<Rigidbody2D>().velocity.x * integer, toPull.GetComponent<Rigidbody2D>().velocity.y * integer);
        }
    }
    void OnDestroy()
    {
        foreach (GameObject particle in particleClones)
        {
            var emission = particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 0f;
            Destroy(particle, 5f);
            emitter.Stop();
        }
    }
}


