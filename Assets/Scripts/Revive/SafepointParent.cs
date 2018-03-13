using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafepointParent : Safepoint 
{
    [Header("Child Settings")]
    [SerializeField]
    private Vector2 childOffset = Vector2.zero;
    public Transform child;

    [Header("Line Settings")]
    [SerializeField]
    private bool showLine = true;
    [SerializeField]
    private Color color = Color.yellow;
    [SerializeField]
    private float lineHeight = 11f; // The distance from 0 on Y to the tip of the rays

    private void OnDrawGizmos()
    {
        child = transform.GetChild(0);                                                                      // Get the child safepoint
        child.transform.position = new Vector2(transform.position.x, -transform.position.y) + childOffset;  // Offset using the set vector

        if (showLine)
        {
            // Draw lines representing the safepoints in the scene
            Gizmos.color = color;
            Gizmos.DrawLine(new Vector3(transform.position.x, lineHeight),
                    new Vector3(transform.position.x, 0f));
            Gizmos.DrawLine(new Vector3(child.position.x, -lineHeight),
                    new Vector3(child.position.x, 0f));
        }
    }

    // Reset the sprite and set to not be the current safepoint, also includes the child
    // Called by the manager when a new pair is found
    public void ResetSafepoint()
    {
        ChangeParticleSystems(0f, 0f);
        child.GetComponent<Safepoint>().ChangeParticleSystems(0f, 0f);
        isCurrent = false;
        child.GetComponent<Safepoint>().isCurrent = false;
    }

    // Make this safepoint the current one, also includes the child
    // Called by the manager when a pair is activated
    public void MakeCurrent()
    {
        playerExited = false;
        timer = 0f;
        isCurrent = true;

        ChangeParticleSystems(0f, 9f);
        child.GetComponent<Safepoint>().ChangeParticleSystems(0f, 9f);

        child.GetComponent<Safepoint>().playerExited = false;
        child.GetComponent<Safepoint>().isCurrent = true;

        FMODUnity.RuntimeManager.PlayOneShot(bothActivated, transform.position);
    }
}
