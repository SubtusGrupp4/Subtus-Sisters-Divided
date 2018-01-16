using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

    Grid grid;

    private int oldIndex;

    private void OnEnable()
    {
        oldIndex = 0;
        grid = (Grid)target;
    }

    [MenuItem("Assets/Create/TileSet")]
    static void CreateTileSet()
    {
        TileSet asset = ScriptableObject.CreateInstance<TileSet>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if(string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if(Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(path), "");
        }
        else
        {
            path += "/";
        }

        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "TileSet.asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        grid.spriteWidth = CreateSlider("Width:", grid.spriteWidth);
        grid.spriteHeight = CreateSlider("Height:", grid.spriteHeight);
        Color newColor = EditorGUILayout.ColorField("Color:", grid.color);
        if(EditorGUI.EndChangeCheck())
        {
            grid.color = newColor;
        }

        // Tile Prefab
        EditorGUI.BeginChangeCheck();
        Transform newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab", grid.tilePrefab, typeof(Transform), false);
        if(EditorGUI.EndChangeCheck())
        {
            grid.tilePrefab = newTilePrefab;
            Undo.RecordObject(target, "Grid Changed");
        }

        // Tile Map
        EditorGUI.BeginChangeCheck();
        TileSet newTileSet = (TileSet)EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false);
        if(EditorGUI.EndChangeCheck())
        {
            grid.tileSet = newTileSet;
            Undo.RecordObject(target, "Grid Changed");
        }

        if(grid.tileSet != null)
        {
            EditorGUI.BeginChangeCheck();
            string[] names = new string[grid.tileSet.prefabs.Length];
            int[] values = new int[names.Length];

            for(int i = 0; i < names.Length; i++)
            {
                names[i] = grid.tileSet.prefabs[i] != null ? grid.tileSet.prefabs[i].name : "";
                values[i] = i;
            }

            int index = EditorGUILayout.IntPopup("Select Tile", oldIndex,names,values);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Grid Changed");
                if(oldIndex != index)
                {
                    oldIndex = index;
                    grid.tilePrefab = grid.tileSet.prefabs[index];

                    //float width = grid.tilePrefab.GetComponent<Renderer>().bounds.size.x;
                    //float height = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;

                    //grid.spriteWidth = width;
                    //grid.spriteHeight = height;
                }
            }
        }
    }

    private float CreateSlider(string labelName, float sliderPosition)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(labelName);
        sliderPosition = EditorGUILayout.Slider(sliderPosition, 8f, 1024f);
        GUILayout.EndHorizontal();

        return sliderPosition;
    }

    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePos = ray.origin;

        // Left click to place prefab
        if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 || e.isMouse && e.type == EventType.MouseDrag && e.button == 0)
        {
            PlaceTile(controlID, e, ray, mousePos);
        }

        // Right click to remove prefab
        if(e.isMouse && e.type == EventType.MouseDown && e.button == 1 || e.isMouse && e.type == EventType.MouseDrag && e.button == 1)
        {
            RemoveTile(controlID, e, mousePos);
        }

        /* If nothing is pressed, lose control (not used)
        if (e.isMouse && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
        */
    }

    private void PlaceTile(int controlID, Event e, Ray ray, Vector3 mousePos)
    {
        GUIUtility.hotControl = controlID;
        e.Use();

        GameObject spawnGO;
        Transform prefab = grid.tilePrefab;

        if (prefab)
        {
            Undo.IncrementCurrentGroup();
            Vector3 aligned = new Vector3(
                Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);

            Transform tileOnPosition = GetTransformFromPosition(aligned);
            if (tileOnPosition != null)
                return;

            spawnGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
            spawnGO.transform.position = aligned;
            spawnGO.transform.parent = grid.transform;

            Undo.RegisterCreatedObjectUndo(spawnGO, "Create " + spawnGO.name);
        }
    }

    private void RemoveTile(int controlID, Event e, Vector3 mousePos)
    {
        GUIUtility.hotControl = controlID;
        e.Use();

        Vector3 aligned = new Vector3(
            Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
            Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);

        Transform tileTransform = GetTransformFromPosition(aligned);
        if (tileTransform != null)
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(tileTransform.gameObject);
        }
    }

    /// <summary>
    /// Might need some optimising, currently checks ALL tiles
    /// </summary>
    private Transform GetTransformFromPosition(Vector3 aligned)
    {
        int i = grid.transform.childCount - 1;
        while(i > 0)
        {
            Transform tileTransform = grid.transform.GetChild(i);
            if(tileTransform.position == aligned)
            {
                return tileTransform;
            }

            i--;
        }

        return null;
    }

}
