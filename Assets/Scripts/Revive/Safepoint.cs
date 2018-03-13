using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class Safepoint : MonoBehaviour 
{
    [Header("Activation")]
    public ParticleSystem triggered;
    public ParticleSystem activated;
    public FlickeringLight flickeringLight;

    [HideInInspector]
    public bool isCurrent = false;

    [Header("Exit Timer")]
    [HideInInspector]
    public bool playerExited = false;
    [SerializeField]
    private float exitTime = 5f;    // Timer time in seconds
    [SerializeField]
    [Range(0f, 1f)]
    protected float timer = 1f;     // Do not change

    [Header("FMOD Events")]
    [SerializeField]
    [FMODUnity.EventRef]
    protected string bothActivated;
    [SerializeField]
    [FMODUnity.EventRef]
    private string oneActivated;

    private void Start()
    {
        ChangeParticleSystems(0f, 0f);
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
                ChangeParticleSystems(0f, 0f);
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
            ChangeParticleSystems(9f, 0f);

            if (transform != SafepointManager.instance.topSafepoint && transform != SafepointManager.instance.botSafepoint)
                    FMODUnity.RuntimeManager.PlayOneShot(oneActivated, transform.position);

            SafepointManager.instance.SetAsActivated(transform);    // Tell the manager that his safepoint is activated
        }
	}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCurrent)                    // If a player exits the trigger
            playerExited = true;                                    // Start the timer
    }

    public void ChangeParticleSystems(float t, float a)
    {
        var emission = triggered.emission;
        emission.rateOverTime = t;
        emission = activated.emission;
        emission.rateOverTime = a;

        if(t > 0.1f)
            ChangeFlickeringLight(0.3f, 0.4f);
        else if(a > 0.1f)
            ChangeFlickeringLight(0.5f, 0.6f);
        else if(t < 0.1f && a < 0.1f)
            ChangeFlickeringLight(0f, 0f);
    }

    private void ChangeFlickeringLight(float min, float max)
    {
        flickeringLight.minIntensity = min;
        flickeringLight.maxIntensity = max;
    }
}
