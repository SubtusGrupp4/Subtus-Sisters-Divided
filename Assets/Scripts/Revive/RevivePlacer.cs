using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make this more robust. Look in more directions, better criterias. Prevent problems occuring from the "CheckAdjacent" collider disabler
public class RevivePlacer : MonoBehaviour 
{
    private Vector2 rayDirection;
    private Vector2 startPos;

    private float moveAmount = 0.01f;
    private float maxDistance = 15f;

    private Transform playerTransform;

    [SerializeField]
    private GameObject reviveSpot;

    private bool passedThrough = false;
    private Vector2 lastSafe;

    // Called by the player that dies
    public void Initialize (Controller player, Transform playerTransform) 
    {
        startPos = transform.position;
        this.playerTransform = playerTransform;

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
        Vector2 rayOrigin = transform.position;

        bool search = true;

        float rayDistance = 0.5f;

        // TODO: Set this correctly. Should for example not detect the players
        LayerMask layer = 1 << 0;

        while (search)
        {
            // Cast a ray looking for blocks
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);

            if (hit.transform != null && !hit.collider.isTrigger && hit.transform.tag != "Player")
            {
                // If over the max distance, or hit portal
                if (rayOrigin.y - startPos.y > maxDistance || hit.transform.tag == "Portal")
                {
                    transform.position = new Vector2(transform.position.x -0.5f, 0f);       // Move slightly to the left. TODO: Better solution?
                    passedThrough = false;
                }
                if (hit.transform != null && hit.transform.tag != "portal") // If it hits something
                    passedThrough = true;                                   // It has passed through a block
            }
            else if(passedThrough)          // After passing through one block and then not finding anything
                search = false;             // This spot is safe

            /* Old solution, doesn't work with the collision remover
            if (hit.transform == null) // If nothing is found, this spot is safe to spawn on. Stop the loop
                search = false;
            else if (Vector2.Distance (rayOrigin, startPos) > maxDistance || hit.transform.tag == "Portal") // If over the max distance, or hit portal
                transform.position = new Vector2 (transform.position.x - 1f, 0f);                           // Move slightly to the left
            */

            transform.position -= new Vector3 (0f, moveAmount);     // Move slightly
            rayOrigin = transform.position;                         // Reset the ray origin to the current position
        }
        if (!search) // If a safe spot is found
        {
            Instantiate (reviveSpot, transform.position, Quaternion.identity);  // Instantiate the respawn pickup
            Destroy (gameObject);                                               // Destroy the current object (the placer)
        }
    }
}