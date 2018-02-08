using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class Checkpoint : MonoBehaviour 
{
    [SerializeField]
    private Vector2 childOffset = Vector2.zero;
    public Transform child;
    private Color color = Color.yellow;

    private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player") 
		{
			if(child == null)
				child = transform.GetChild(0);

            PushCheckpointManager();
        }
	}

    public virtual void PushCheckpointManager() 
    {
        CheckpointManager.instance.Checkpoint(transform, child.transform);
    }

	private void OnDrawGizmos() 
	{
        child = transform.GetChild(0);
        child.transform.position = new Vector2(transform.position.x, -transform.position.y) + childOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(transform.position.x, -11f),
                new Vector3(transform.position.x, 11f));
	}
}
