using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make this more robust. Look in more directions, better criterias. Prevent problems occuring from the "CheckAdjacent" collider disabler
public class RevivePlacer : MonoBehaviour 
{
    private Vector2 rayDirection;

    private float moveAmount = -0.25f;
    private float maxDistance = 15f;
    private float rayDistance = 2.25f;

    [SerializeField]
    private GameObject reviveSpot;

    private bool passedThrough = false;
    private Vector2 lastSafe;

    [SerializeField]
    private bool useDebugDelay = true;

    private bool search = true;

    // Called by the player that dies
    public void Initialize (Controller player) 
    {
        // Change the direction based on what player died
        // If player one died, search up
        if (player == Controller.Player1)
        {
            rayDirection = Vector2.up;
            rayDistance = 3.25f;
        }
        else
        {
            rayDirection = Vector2.down;
            moveAmount = -moveAmount;
        }

        Raycast ();
    }

    private void Raycast () 
    {
        if (useDebugDelay)
            StartCoroutine(SlowRaycast());
        else
        {
            while (search)
            {
                transform.position += new Vector3(0f, moveAmount);

                if (Mathf.Abs(transform.position.y) > maxDistance)
                {
                    transform.position = new Vector2(transform.position.x - 0.5f, 0f);
                    passedThrough = false;
                }

                Vector2 rayOrigin = transform.position;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);

                if (hit.transform != null)
                {
                    // Criterias for what is hit
                    if (hit.transform.tag == "Portal")
                    {
                        transform.position = new Vector2(transform.position.x - 0.5f, 0f);
                        passedThrough = false;
                        continue;
                    }

                    if (hit.transform.tag == "Player")
                        search = true;

                    if (hit.collider.isTrigger)
                    {
                        if (hit.transform.tag != "Player" || hit.transform.tag == "ReviveBlock")
                        {
                            passedThrough = false;
                            continue;
                        }
                    }

                    if (hit.transform.position.y < 0f && moveAmount > 0f)
                        continue;
                    else if (hit.transform.position.y > 0f && moveAmount < 0f)
                        continue;

                    passedThrough = true;
                }

                if (hit.transform == null)
                    if (passedThrough == true)
                        search = false;
            }

            if(!search)
            {
                float spawnHeight = rayDistance / 2f;
                if (moveAmount < 0f)
                    spawnHeight = -spawnHeight;

                Instantiate(reviveSpot, transform.position - new Vector3(0f, spawnHeight), Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    // Rewrite
    private IEnumerator SlowRaycast()
    {
        transform.position += new Vector3(0f, moveAmount);

        if (Mathf.Abs(transform.position.y) > maxDistance)
        {
            transform.position = new Vector2(transform.position.x - 0.5f, 0f);
            passedThrough = false;
        }

        Vector2 rayOrigin = transform.position;


        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
        Debug.Log("Raycasting...");

        if(hit.transform != null)
        {
            Debug.Log("Hit: " + hit.transform.name + ", pos: " + hit.transform.position);
            // Criterias for what is hit
            if(hit.transform.tag == "Portal")
            {
                Debug.Log("Hit portal");
                transform.position = new Vector2(transform.position.x - 0.5f, 0f);
                passedThrough = false;
                StartCoroutine(SlowRaycast());
                yield break;
            }
            if(hit.collider.isTrigger)
            {
                Debug.Log("Hit trigger");
                StartCoroutine(SlowRaycast());
                yield break;
            }
            if(hit.transform.position.y < 0f && moveAmount > 0f)
            {
                Debug.Log("Hit block on the wrong side (down)");
                StartCoroutine(SlowRaycast());
                yield break;
            }
            if(hit.transform.position.y > 0f && moveAmount < 0f)
            {
                Debug.Log("Hit block on the wrong side (up)");
                StartCoroutine(SlowRaycast());
                yield break;
            }

            Debug.Log("Passed through = true");
            passedThrough = true;
        } 

        if(hit.transform == null)
        {
            Debug.Log("Hit nothing");
            if(passedThrough == true)
            {
                Debug.Log("Had passed through. Spawning thing");
                Instantiate(reviveSpot, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        yield return new WaitForSeconds(0.05f);
        StartCoroutine(SlowRaycast());
    }
}

/*
    private void Raycast () 
    {
        if (useDebugDelay)
            StartCoroutine(SlowRaycast());
        else
        {
            Vector2 rayOrigin = transform.position;
            bool search = true;
            float rayDistance = 0.5f;

            // TODO: Set this correctly. Should for example not detect the players
            //LayerMask layer = 1 << 0;

            while (search)
            {
                if(Mathf.Abs(transform.position.y) > maxDistance)
                {
                    transform.position = new Vector2(transform.position.x - 0.5f, 0f);
                    passedThrough = false;
                }

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);

                if (hit.transform != null)
                {
                    if (hit.transform.tag != "Player" && moveAmount > 0f && hit.transform.position.y > 0f || hit.transform.tag != "Player" && moveAmount < 0f && hit.transform.position.y < 0f)
                    {

                        if (hit.transform.tag == "Portal")
                        {
                            transform.position = new Vector2(transform.position.x - 0.5f, 0f);
                            passedThrough = false;
                        }
                        if (hit.transform != null && !hit.collider.isTrigger && hit.transform.tag != "Portal")
                            passedThrough = true;
                    }
                }
                else if (passedThrough)
                    search = false;

                transform.position -= new Vector3(0f, moveAmount);
                rayOrigin = transform.position;
            }
            if (!search)
            {
                Instantiate(reviveSpot, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    Vector2 rayOrigin = transform.position;
    bool search = true;
    float rayDistance = 0.5f;

    // TODO: Set this correctly. Should for example not detect the players
    //LayerMask layer = 1 << 0;

    if (Mathf.Abs(transform.position.y) > maxDistance)
    {
        transform.position = new Vector2(transform.position.x - 0.5f, 0f);
        passedThrough = false;
    }

    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
    Color color = hit ? Color.green : Color.red;
    Debug.DrawRay(rayOrigin, rayDirection, color);

    if (hit.transform != null)
    {
        if (hit.transform.tag == "Portal")
        {
            transform.position = new Vector2(transform.position.x - 0.5f, 0f);
            passedThrough = false;
        }

        if (hit.transform.tag != "Player")
            if (moveAmount < 0f && hit.transform.position.y < 0f || moveAmount > 0f && hit.transform.position.y > 0f)
                if (!hit.collider.isTrigger && hit.transform.tag != "Portal")
                    passedThrough = true;
    }
    else if (passedThrough)
        search = false;

    transform.position -= new Vector3(0f, moveAmount);

    yield return new WaitForSeconds(0.01f);

    if (!search)
    {
        Instantiate(reviveSpot, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    else
        Raycast();
*/
