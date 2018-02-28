using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpot : MonoBehaviour
{
    [SerializeField]
    private float bounceAmount = 1f;
    [SerializeField]
    private float bounceInterval = 3f;
    private Vector2 startPos;
    /*
    Transform playerTransform;

    public void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;     // Get what player should be revived
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerTransform.GetComponent<PlayerController>().Ressurect();   // Revive the player
            Destroy(gameObject);                                            // Destroy self
        }
    }
    */

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x, startPos.y + (Mathf.Sin(Time.time * bounceInterval) * bounceAmount));
    }
}
