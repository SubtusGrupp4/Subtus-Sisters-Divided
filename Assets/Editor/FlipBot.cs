using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FlipBot : Editor
{
    [MenuItem("4K Studios Tools/Flip Bot")]
	static void DoFlipBot ()
    {
        GameObject[] gameObjects;
        gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject go in gameObjects)
        {
            if(go.transform.position.y < -0.1f)
            {
                go.transform.position = new Vector2(go.transform.position.x, (-go.transform.position.y - 11f));
                go.transform.localScale = new Vector3(1f, -go.transform.localScale.y, 1f);
            }
        }
	}
}
