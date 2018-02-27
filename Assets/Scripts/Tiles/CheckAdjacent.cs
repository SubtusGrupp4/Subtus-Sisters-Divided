using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAdjacent : MonoBehaviour 
{
	public int DoCheck(int index) 
	{
        Debug.Log("Checking...");
        Vector2 origin = transform.position;
		Vector2 rayDirection = Vector2.up;
		float rayDistance = 0.2f;
		int hits = 0;

        bool up = false;
        bool right = false;
        bool down = false;
        bool left = false;

        for(int i = 0; i < 4; i++) 
		{
			RaycastHit2D hit = Physics2D.Raycast(origin + rayDirection, rayDirection, rayDistance);
			
            if(hit.transform != null && hit.transform.GetComponent<CheckAdjacent>() != null)
				hits++;

            if (rayDirection == Vector2.up)
            {
                rayDirection = Vector2.right;
				if(hit.transform != null && hit.transform.GetComponent<CheckAdjacent>() != null)
                    up = true;
            }
            else if (rayDirection == Vector2.right)
            {
                rayDirection = Vector2.down;
                if (hit.transform != null && hit.transform.GetComponent<CheckAdjacent>() != null)
                    right = true;
            }
            else if(rayDirection == Vector2.down)
            {
                rayDirection = Vector2.left;
                if (hit.transform != null && hit.transform.GetComponent<CheckAdjacent>() != null)
                    down = true;
            }
			else 
			{
                if (hit.transform != null && hit.transform.GetComponent<CheckAdjacent>() != null)
                    left = true;
			}
        }

		if(GetComponent<RandomDecorations>() != null)
            SetDecorationMargins(up, right, down, left);

        if(hits == 4) 
		{
            return -1;
        }
		else
            return index;
    }

	public void RestoreColliders() 
	{
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
                Debug.Log("Enabled colliders");
            }
        }

        DestroyImmediate(GetComponent<SetParentTo>());
    }

	private void SetDecorationMargins(bool up, bool right, bool down, bool left) 
	{
        GetComponent<RandomDecorations>().SetMargins(up, right, down, left);
    }
}