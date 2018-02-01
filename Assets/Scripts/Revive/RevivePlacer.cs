using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivePlacer : MonoBehaviour {

    private Vector2 rayDirection;
    private Vector2 startPos;

    private float moveAmount = 0.01f;
    private float maxDistance = 15f;

    private Transform playerTransform;

    [SerializeField]
    private GameObject reviveSpot;

    public void Initialize(Controller player, Transform playerTransform)
    {
        startPos = transform.position;
        this.playerTransform = playerTransform;

        if (player == Controller.Player1)
        {
            rayDirection = Vector2.up;
        }
        else
        {
            moveAmount = -moveAmount;
            rayDirection = Vector2.down;
        }

        Raycast();
    }

    private void Raycast()
    {
        Vector2 rayOrigin = transform.position;

        bool search = true;

        float rayDistance = 0.5f;

        LayerMask layer = 1 << 0;

        while (search)
        {
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);
            Color color = hit ? Color.green : Color.red;
            Debug.DrawRay(rayOrigin, rayDirection, color);

            if (hit.transform == null)
                search = false;
            else if (Vector2.Distance(rayOrigin, startPos) > maxDistance || hit.transform.tag == "Portal")
            {
                transform.position = new Vector2(transform.position.x - 1f, 0f);
            }

            transform.position -= new Vector3(0f, moveAmount);
            rayOrigin = transform.position;
        }
        if(!search)
        {
            GameObject spot = Instantiate(reviveSpot, transform.position, Quaternion.identity);
            spot.GetComponent<ReviveSpot>().Initialize(playerTransform);
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedSearch()
    {
        yield return new WaitForSeconds(0.1f);
        Raycast();
    }
}
