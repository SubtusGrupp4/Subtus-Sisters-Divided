using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravitationBomb : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField]
    private int range;
    [SerializeField]
    private int explosionForce;
    [SerializeField]
    private float duration;

	[Header("Particles")]
	[SerializeField]
	private GameObject[] particles;

    [Header("Tags from objects to pull")]
    [SerializeField]
    [GiveTag]
    private string[] tagNames;

	private GameObject[] ignoreObject;
    private List<GameObject> pullObjects;
    private Rigidbody2D rb;
    private bool targetting;
    private bool antiGravitationActivated;
    private string activateBomb = "Fire_Bomb_C1";

    [Header("FMOD Audio")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string thrown;
    [SerializeField]
    [FMODUnity.EventRef]
    private string activate;

    void Start()
    {
        ignoreObject = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject IO in ignoreObject)
			Physics2D.IgnoreCollision (IO.GetComponent<Collider2D> (), this.GetComponent<Collider2D>(), true);
		
        rb = GetComponent<Rigidbody2D>();
        pullObjects = new List<GameObject>();
        antiGravitationActivated = false;
        targetting = false;

        FMODUnity.RuntimeManager.PlayOneShot(thrown, transform.position);

        if (transform.position.y > 0f)
            rb.gravityScale = Mathf.Abs(rb.gravityScale);
        else
            rb.gravityScale = -Mathf.Abs(rb.gravityScale);
    }

    void Update()
    {
		
        ActivateAntiGravitationBomb();

    }

    void ActivateAntiGravitationBomb()
    {
		if ((Input.GetAxis(activateBomb) != 0f || Input.GetKeyDown(KeyCode.O)))
        {
			foreach (GameObject particle in particles) 
			{
				GameObject clone = Instantiate (particle, transform.position, Quaternion.identity);
				Destroy (clone, 5f);
			}
            targetting = true;
            TargetInRange();
            antiGravitationActivated = true;
            AntiGravitationPull();
            rb.bodyType = RigidbodyType2D.Static;

            FMODUnity.RuntimeManager.PlayOneShot(activate, transform.position);
            GetComponent<AudioOnCollision>().isActive = false;

            Destroy(gameObject);
        }
        Destroy(gameObject, duration);
    }

    void TargetInRange()
    {
        if (targetting)
        {
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
        }
    }

    void AntiGravitationPull()
    {
        if (antiGravitationActivated)
        {
            foreach (GameObject toPull in pullObjects)
            {
                toPull.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                ShockWave(toPull, explosionForce);
            }
        }
    }

    void ShockWave(GameObject obj, float push)
    {
        float distance = Vector3.Distance(transform.position, obj.transform.position);
        float xDirection = (obj.transform.position.x - transform.position.x);
        float yDirection = (obj.transform.position.y - transform.position.y);
        Vector2 direction = new Vector2(xDirection, yDirection);

        Vector2 theForce = direction.normalized * obj.GetComponent<Rigidbody2D>().mass * push -
            direction.normalized * push * obj.GetComponent<Rigidbody2D>().mass / -distance;

        if (direction.normalized.y < 0)
            theForce = new Vector2(theForce.x, push * 0.5f);

        if (obj.GetComponent<AIMovement>())
        {
            obj.GetComponent<AIMovement>().Stun(0.5f);
            obj.GetComponent<AIMovement>().Freeze(true, 0);
        }

        obj.GetComponent<Rigidbody2D>().AddForce(theForce);
        //Debug.Log (obj.name + theForce + distance + direction);


    }


}