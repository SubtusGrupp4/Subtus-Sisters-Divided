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
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin, rayDirection, rayDistance, layer);
            Color color = hit ? Color.green : Color.red;
            Debug.DrawRay (rayOrigin, rayDirection, color);

            if (hit.transform == null) // If nothing is found, this spot is safe to spawn on. Stop the loop
                search = false;
            else if (Vector2.Distance (rayOrigin, startPos) > maxDistance || hit.transform.tag == "Portal") // If over the max distance, or hit portal
                transform.position = new Vector2 (transform.position.x - 1f, 0f);                           // Move slightly to the left

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