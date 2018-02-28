using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make this more robust. Look in more directions, better criterias. Prevent problems occuring from the "CheckAdjacent" collider disabler
public class RevivePlacer : MonoBehaviour 
{
    private Vector2 rayDirection;

    private float moveAmount = 0.05f;
    private float maxDistance = 15f;

    [SerializeField]
    private GameObject reviveSpot;

    private bool passedThrough = false;
    private Vector2 lastSafe;

    [SerializeField]
    private bool useDebugDelay = true;

    // Called by the player that dies
    public void Initialize (Controller player) 
    {
        // Change the direction based on what player died
        if (player == Controller.Player1)
            rayDirection = Vector2.up;
        else 
        {
            moveAmount = -moveAmount;
            rayDirection = Vector2.down;
        }

        Raycast ();
    }

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
                // Cast a ray looking for blocks
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);

                if (hit.transform != null)  // If it hits something
                {
                    // Check if player or block on opposite side
                    if (hit.transform.tag != "Player" && moveAmount < 0f && hit.transform.position.y > 0f || hit.transform.tag != "Player" && moveAmount > 0f && hit.transform.position.y < 0f)
                    {

                        // If over the max distance, or hit portal
                        if (Mathf.Abs(rayOrigin.y) > maxDistance || hit.transform.tag == "Portal")
                        {
                            transform.position = new Vector2(transform.position.x - 0.5f, 0f);       // Move slightly to the left. TODO: Better solution?
                            passedThrough = false;
                        }
                        if (hit.transform != null && !hit.collider.isTrigger && hit.transform.tag != "Portal") // If it hits something
                            passedThrough = true;// It has passed through a block
                    }
                }
                else if (passedThrough)  // After passing through one block and then not finding anything
                    search = false;     // This spot is safe

                transform.position -= new Vector3(0f, moveAmount);     // Move slightly
                rayOrigin = transform.position;                         // Reset the ray origin to the current position
            }
            if (!search) // If a safe spot is found
            {
                Instantiate(reviveSpot, transform.position + new Vector3(0f, 0.5f), Quaternion.identity);  // Instantiate the respawn pickup
                Destroy(gameObject);                                               // Destroy the current object (the placer)
            }
        }
    }

    private IEnumerator SlowRaycast()
    {
        Vector2 rayOrigin = transform.position;
        bool search = true;
        float rayDistance = 0.5f;

        // TODO: Set this correctly. Should for example not detect the players
        //LayerMask layer = 1 << 0;

        // Cast a ray looking for blocks
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
        Color color = hit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, rayDirection, color);
        Debug.Log("Casting ray");

        if (hit.transform != null)  // If it hits something
        {
            Debug.Log("Hit something");
            if (hit.transform.tag != "Player" && moveAmount < 0f && hit.transform.position.y > 0f || hit.transform.tag != "Player" && moveAmount > 0f && hit.transform.position.y < 0f)  // And it isn't the player
            {
                Debug.Log("Not the player or a block on the wrong side!");
                // If over the max distance, or hit portal
                if (Mathf.Abs(rayOrigin.y) > maxDistance || hit.transform.tag == "Portal")
                {
                    Debug.Log("Dangerous; Move to the left");
                    transform.position = new Vector2(transform.position.x - 0.5f, 0f);       // Move slightly to the left. TODO: Better solution?
                    passedThrough = false;
                }
                if (hit.transform != null && !hit.collider.isTrigger && hit.transform.tag != "Portal") // If it hits something
                {
                    Debug.Log("passedThrough = true");
                    passedThrough = true;// It has passed through a block
                }
            }
        }
        else if (passedThrough)  // After passing through one block and then not finding anything
        {
            Debug.Log("Found a safe spot at " + transform.position + ", search = false");
            search = false;     // This spot is safe
        }

        Debug.Log("Moving slightly up");
        transform.position -= new Vector3(0f, moveAmount);     // Move slightly

        yield return new WaitForSeconds(0.01f);

        if (!search) // If a safe spot is found
        {
            Debug.Log("Spawning revive spot");
            Instantiate(reviveSpot, transform.position + new Vector3(0f, 0.5f), Quaternion.identity);  // Instantiate the respawn pickup
            Destroy(gameObject);                                               // Destroy the current object (the placer)
        }
        else
            Raycast();
    }
}