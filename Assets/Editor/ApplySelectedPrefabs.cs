using UnityEditor;
using UnityEngine;
 
public class ApplySelectedPrefabs : EditorWindow
{
    [MenuItem ("Tools/Prefabs/Revert all selected prefabs")]
    static void ResetPrefabs()
    {
		GameObject[] selection = Selection.gameObjects;

		if (selection.Length > 0)
		{
			for (int i = 0; i < selection.Length; i++) 
			{
            	PrefabUtility.ResetToPrefabState(selection[i]);
                Debug.Log("Reset to prefab state");
            }
    	} 
		else 
		{
         	Debug.Log("Cannot revert to prefab - nothing selected");
     	}
    }

    [MenuItem("Tools/Prefabs/Replace all selected with prefab")]
    static void ReplacePrefabs()
    {
        Transform[] selection = Selection.transforms;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                Transform instance = (Transform)PrefabUtility.InstantiatePrefab(PrefabUtility.GetPrefabParent(selection[i]));
                instance.position = selection[i].position;
                instance.rotation = selection[i].rotation;
                instance.localScale = selection[i].localScale;
                instance.parent = selection[i].parent;
                instance.GetComponent<SpriteRenderer>().color = selection[i].GetComponent<SpriteRenderer>().color;
                instance.GetComponent<SpriteRenderer>().sortingOrder = selection[i].GetComponent<SpriteRenderer>().sortingOrder;
                instance.GetComponent<SpriteRenderer>().flipX = selection[i].GetComponent<SpriteRenderer>().flipX;
                instance.GetComponent<SpriteRenderer>().flipY = selection[i].GetComponent<SpriteRenderer>().flipY;
                DestroyImmediate(selection[i].gameObject);

                //PrefabUtility.ReplacePrefab(selection[i], PrefabUtility.GetPrefabParent(selection[i]));
                Debug.Log("Replaced with prefab");
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }

    [MenuItem("Tools/Prefabs/Apply all selected prefabs")]
    static void ApplyPrefabs()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                PrefabUtility.ReplacePrefab(selection[i], PrefabUtility.GetPrefabParent(selection[i]), ReplacePrefabOptions.ConnectToPrefab);
                Debug.Log("Applied selected object");
            }
        }
        else
        {
            Debug.Log("Cannot apply prefab - nothing selected");
        }
    }
}