using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : AIMovement
{
    [Header("Police")]
    private float startSpeed;
    public float engagedSpeed;

    public float attentionSpan;
    private float attentionTimer;

    protected override void Awake()
    {
        base.Awake();
       

    }

    private void Start()
    {
        startSpeed = speed;

    }


    // Use this for initialization
    protected override void Update()
    {
        if (!engaged)
            CheckEngagement();

        if (engaged)
            CheckDisEngagement();

        CheckFalling();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    protected override void CheckEngagement()
    {
        // base.CheckEngagement();
        bAnim.ToggleWalk(true);

        {
            allTargets.Clear(); // clear the target list, incase another target is added or removed etc...
            if (engageOn.Length >= 1)
                for (int i = 0; i < engageOn.Length; i++)
                    allTargets.AddRange(GameObject.FindGameObjectsWithTag(engageOn[i]));

            float tempDistance = aggroRange;
            GameObject tempTarget = null;

            for (int i = 0; i < allTargets.Count; i++)
            {
                float distance = Vector2.Distance(transform.position, allTargets[i].transform.position);

                if (distance > aggroRange)
                    continue;

                // IF IN RANGE
                if (distance <= tempDistance)
                {
                    bool blocked = false;
                    // RAYCAST
                    objHit = Physics2D.RaycastAll
                        (transform.position,
                        ((Vector2)allTargets[i].transform.position - (Vector2)transform.position).normalized,
                        tempDistance + 0);


                    Debug.DrawRay(transform.position,
                        ((Vector2)allTargets[i].transform.position - (Vector2)transform.position).normalized,
                        Color.cyan);


                    // ALL WALKS ABLE OBJECTS
                    for (int l = 0; l < walkOn.Length; l++)
                    {
                        for (int j = 0; j < objHit.Length; j++)
                        {
                            if (objHit[j].transform.tag == walkOn[l])
                                blocked = true;
                        }
                    }

                    if (blocked)
                        continue;

                    // CHECK IF YOU'RE FACING THE PLAYER
                    Vector2 facing = Facing();

                    if ((((Vector2)allTargets[i].transform.position - (Vector2)transform.position).x >= 0 ? 1f : -1f) == facing.x)
                    {
                        // WE'RE NOW FACING THE PLAYA

                        tempDistance = distance;
                        tempTarget = allTargets[i];
                        // EQUALS WE GET TO TARGET HIM YES
                    }
                }
            }
            target = tempTarget;
            if (target != null)
            {
                target = tempTarget;
                engaged = true;
                currentState = engagedState;
                speed = engagedSpeed;

                
            }
           

        }
    }

    private void CheckDisEngagement()
    {
        bAnim.ToggleWalk(false);
        if (attentionTimer >= attentionSpan || target.activeInHierarchy != true)
        {
            currentState = startState;
            Bounce();
            engaged = false;
            speed = startSpeed;
           
        }
     

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // IF IN RANGE
        {
            bool blocked = false;
            // RAYCAST
            objHit = Physics2D.RaycastAll
                (transform.position,
                ((Vector2)target.transform.position - (Vector2)transform.position).normalized,
                distance + 0);


            Debug.DrawRay(transform.position,
                ((Vector2)target.transform.position - (Vector2)transform.position).normalized,
                Color.magenta);


            // ALL WALKS ABLE OBJECTS
            for (int l = 0; l < walkOn.Length; l++)
            {
                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == walkOn[l])
                        blocked = true;
                }
            }

            if (blocked)
            {
                // ATTENTION SPAN, NEEDS TO COUNT UP WHILE It'S NOT IN LINE OF SIGHT
                // WHEN IT REACHES FULL THEN WE DISENGAGE
                attentionTimer += Time.deltaTime;


                // TURN
                // STATE change to startstate 
            }
            else
            {
                attentionTimer = 0;
                // RESET ATTENTIONSPAN
            }

        }

    }
}
