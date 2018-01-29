using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    [GiveTag]
    [SerializeField]
    private string[] tags;
    private Transform pickedUp;
    [SerializeField]
    private Transform hitTransform;

    private Rigidbody2D rb;
    private bool goingRight = true;

    private Vector2 grabPosition;

    private bool isPickedUp = false;
    private bool pickupEnabled = true;

    [Header("Ray Settings")]
    [SerializeField]
    private float rayOffsetX = 0.6f;
    [SerializeField]
    private float rayOffsetY = 0.6f;
    [SerializeField]
    private float rayDistance = 0.5f;

    [Header("Grabbing")]
    [SerializeField]
    private Vector3 grabRight;
    [SerializeField]
    private Vector3 grabLeft;

    [Space]
    [SerializeField]
    private float throwDistance = 2.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grabPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // TODO: Replace with last direction in PlayerController
        if (rb.velocity.x > 0.1f)
            goingRight = true;
        else if (rb.velocity.x < -0.1f)
            goingRight = false;

        Vector2 rayOrigin = Vector2.zero;
        if (goingRight)
            rayOrigin = transform.position + new Vector3(rayOffsetX, -rayOffsetY);
        else
            rayOrigin = transform.position + new Vector3(-rayOffsetX, -rayOffsetY);

        Vector2 rayDirection = Vector2.zero;
        if (goingRight)
        {
            rayDirection = Vector2.right;
            grabPosition = transform.position + grabRight;
        }
        else
        {
            rayDirection = Vector2.left;
            grabPosition = transform.position + grabLeft;
        }

        // TODO: Set this correctly
        LayerMask layer = 1 << 0;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);
        Color color = hit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, rayDirection, color);

        if (hit.transform != null)
        {
            foreach (string tag in tags)
            {
                if (hit.transform.tag == tag)
                    hitTransform = hit.transform;
            }
        }
        else if(!isPickedUp)
            hitTransform = null;

        if (isPickedUp)
        {
            pickedUp.position = grabPosition;
            DisableOnPickup();
        }
        else if (!pickupEnabled)
        {
            EnableOnPickup(rayDirection);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && hitTransform != null)
        {
            isPickedUp = !isPickedUp;
            pickedUp = hitTransform;
        }
    }

    private void DisableOnPickup()
    {
        pickedUp.GetComponent<Rigidbody2D>().isKinematic = true;
        Collider2D[] colliders = pickedUp.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        pickupEnabled = false;
    }

    private void EnableOnPickup(Vector2 direction)
    {
        pickedUp.GetComponent<Rigidbody2D>().isKinematic = false;
        Collider2D[] colliders = pickedUp.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = true;

        pickedUp.GetComponent<Rigidbody2D>().velocity = (direction + rb.velocity) * throwDistance;

        pickupEnabled = true;
    }
}
