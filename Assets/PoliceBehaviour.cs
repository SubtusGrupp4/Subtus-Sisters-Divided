using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : AIMovement
{

    // Use this for initialization
    protected override void Update()
    {
        CheckEngagement();

        CheckDisEngagement();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    protected override void CheckEngagement()
    {
        base.CheckEngagement();


        {
            allTargets.Clear(); // clear the target list, incase another target is added or removed etc...
            if (EngageOn.Length >= 1)
                for (int i = 0; i < EngageOn.Length; i++)
                    allTargets.AddRange(GameObject.FindGameObjectsWithTag(EngageOn[i]));

            float tempDistance = AggroRange;
            GameObject tempTarget = null;

            for (int i = 0; i < allTargets.Count; i++)
            {
                float distance = Vector2.Distance(transform.position, allTargets[i].transform.position);

                if (distance > AggroRange)
                    continue;

                // IF IN RANGE
                if (distance <= tempDistance)
                {
                    bool blocked = false;
                    // RAYCAST
                    objHit = Physics2D.RaycastAll
                        (transform.position,
                        ((Vector2)allTargets[i].transform.position - (Vector2)transform.position).normalized,
                        tempDistance + 3f);


                    Debug.DrawRay(transform.position,
                        ((Vector2)allTargets[i].transform.position - (Vector2)transform.position).normalized,
                        Color.cyan);


                    // ALL WALKS ABLE OBJECTS
                    for (int l = 0; l < WalkOn.Length; l++)
                    {
                        for (int j = 0; j < objHit.Length; j++)
                        {
                            if (objHit[j].transform.tag == WalkOn[l])
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
                currentState = EngagedState;
            }
            else
            {
                engaged = false;
                currentState = StartState;
            }
        }
    }

    private void CheckDisEngagement()
    {

    }
}
