using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class Safepoint : MonoBehaviour 
{
    [Header("Activation")]
    [SerializeField]
    private Sprite collectedSprite;
    protected Sprite startSprite;

    public bool isCurrent = false;

    protected SpriteRenderer sr;

    [Header("Exit Timer")]
    public bool playerExited = false;
    [SerializeField]
    private float exitTime = 5f;
    [SerializeField]
    [Range(0f, 1f)]
    protected float timer = 1f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startSprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {
        // If the player has exited the trigger and the safepoint is not the current spawnpoint
        if (playerExited && !isCurrent)
        {
            // Start a timer towards deactivating the safepoint
            if (timer > 0f)
                timer -= Time.deltaTime / exitTime;
            else
            {
                // When it is done, deactivate the safepoint and change the sprite
                SafepointManager.instance.SetAsDeactivated(transform);
                sr.sprite = startSprite;
                timer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player" && !isCurrent)                     // If a player enters the trigger
		{
            playerExited = false;                                   // Stop the timer
            timer = 1f;                                             // Reset the timer
            sr.sprite = collectedSprite;                            // Change the sprite
            SafepointManager.instance.SetAsActivated(transform);    // Tell the manager that his safepoint is activated
        }
	}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCurrent)                    // If a player exits the trigger
            playerExited = true;                                    // Start the timer
    }
}
