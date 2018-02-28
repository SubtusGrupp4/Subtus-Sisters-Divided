using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SortAll : Editor
{
    [MenuItem("4K Studios Tools/Sort All")]
    private static void SetSorting ()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject go in gameObjects)
        {
            if (go.GetComponent<SpriteRenderer>() != null)
            {
                go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(go.transform.position.x * go.transform.position.y);
            }
        }
	}
}
