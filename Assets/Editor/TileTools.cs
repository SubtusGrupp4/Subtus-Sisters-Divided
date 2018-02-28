using UnityEditor;
using UnityEngine;
 
public class ApplySelectedPrefabs : EditorWindow
{
    [MenuItem ("4K Studios Tools/Tiles/Switch Sprite")]
    static void SwitchSprite()
    {
		Transform[] selection = Selection.transforms;

		if (selection.Length > 0)
		{
			for (int i = 0; i < selection.Length; i++) 
			{
                CombinedTile dspr = selection[i].GetComponent<CombinedTile>();
                if(dspr != null) 
                {
                    dspr.SwitchSprites();
                    Debug.Log("Switched sprites on selected tiles");
                }
            }
    	} 
		else 
		{
         	Debug.Log("Nothing selected");
     	}
    }

    [MenuItem("4K Studios Tools/Tiles/Flip Tile Y")]
    static void FlipTileY()
    {
        Transform[] selection = Selection.transforms;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                CombinedTile dspr = selection[i].GetComponent<CombinedTile>();
                if (dspr != null)
                {
                    dspr.FlipY();
                    Debug.Log("Flipped tile on Y on selected tiles");
                }
            }
        }
        else
        {
            Debug.Log("Nothing selected");
        }
    }
}