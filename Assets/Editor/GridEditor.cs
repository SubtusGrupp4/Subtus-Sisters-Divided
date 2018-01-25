using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(TileGrid))]
public class GridEditor : Editor {

    TileGrid grid;

    private int oldIndex;

    private void OnEnable()
    {
        oldIndex = 0;
        grid = (TileGrid)target;
    }

    [MenuItem("Assets/Create/TileSet")]
    static void CreateTileSet()
    {
        TileSet asset = CreateInstance<TileSet>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if(string.IsNullOrEmpty(path))
            path = "Assets";
        else if(Path.GetExtension(path) != "")
            path = path.Replace(Path.GetFileName(path), "");
        else
            path += "/";

        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "TileSet.asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public override void OnInspectorGUI()
    {
        grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
        grid.useGrid = EditorGUILayout.Toggle("Use Grid", grid.useGrid);
        grid.snapPreview = grid.useGrid ? true : false;

        grid.drag = EditorGUILayout.Toggle("Enable Drag", grid.drag);
        grid.overlap = EditorGUILayout.Toggle("Enable Overlap", grid.overlap);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        grid.showGrid = EditorGUILayout.Toggle("Show Grid", grid.showGrid);
        grid.width = MinFloat("Width", grid.width, 0.01f);
        grid.height = MinFloat("Height", grid.height, 0.01f);
        Color newColor = EditorGUILayout.ColorField("Grid Color", grid.color);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mouse Preview", EditorStyles.boldLabel);
        grid.showPreview = EditorGUILayout.Toggle("Show Preview", grid.showPreview);
        if (grid.showPreview)
        {
            grid.snapPreview = EditorGUILayout.Toggle("Snap to Grid", grid.snapPreview);
            grid.previewTransparency = CreateSlider("Preview Transparency", grid.previewTransparency, 0f, 1f);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Settings", EditorStyles.boldLabel);
        grid.tileColor = EditorGUILayout.ColorField("Tile Color:", grid.tileColor);
        grid.rotationZ = EditorGUILayout.FloatField("Rotaton", grid.rotationZ);
        grid.hideInHierarchy = EditorGUILayout.Toggle(new GUIContent("Hide in Hierarchy", "Placed tiles will be invisible in the Hierarchy window. They are still visible in the debug variables."), grid.hideInHierarchy);

        if (EditorGUI.EndChangeCheck())
        {
            grid.color = newColor;
            if(grid.mousePreview != null)
                grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
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
                    grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
                    grid.rotationZ = 0f;

                    float width = grid.tilePrefab.GetComponent<Renderer>().bounds.size.x;
                    float height = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;

                    grid.width = width;
                    grid.height = height;

                    grid.mousePreview.GetComponent<SpriteRenderer>().sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
                    grid.mousePreview.transform.localScale = grid.tilePrefab.transform.localScale;
                    grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
                }
            }
            if(grid.sprite != null)
                EditorGUILayout.ObjectField(new GUIContent("Tile Sprite", "READ-ONLY. Changing this does not affect anything."), grid.sprite, typeof(Sprite), false);
        }

        EditorGUILayout.Space();
        grid.debug = EditorGUILayout.Toggle(new GUIContent("Display Debug", "Displays debug variables. Useful for debugging."), grid.debug);
        if (grid.debug)
            base.OnInspectorGUI();
    }

    private float MinFloat(string labelName, float value, float min)
    {
        value = EditorGUILayout.FloatField(labelName, value);
        if (value < min)
            value = min;
        return value;
    }

    private float CreateSlider(string labelName, float sliderPosition, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(labelName);
        sliderPosition = EditorGUILayout.Slider(sliderPosition, min, max);
        GUILayout.EndHorizontal();

        return sliderPosition;
    }

    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePos = ray.origin;

        if (grid.drag)
        {
            // Left click to place prefab
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 || e.isMouse && e.type == EventType.MouseDrag && e.button == 0)
                PlaceTile(controlID, e, ray, mousePos);

            // Right click to remove prefab
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 1 || e.isMouse && e.type == EventType.MouseDrag && e.button == 1)
                RemoveTile(controlID, e, mousePos);
        }
        else
        {
            // Left click to place prefab
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0)
                PlaceTile(controlID, e, ray, mousePos);

            // Right click to remove prefab
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 1)
                RemoveTile(controlID, e, mousePos);
        }

        switch (e.type)
        {
            case EventType.KeyDown:
                {
                    if (Event.current.keyCode == (KeyCode.Keypad1))
                        grid.tileColor = Color.white;
                    if (Event.current.keyCode == (KeyCode.Keypad2))
                        grid.tileColor = Color.red;
                    if (Event.current.keyCode == (KeyCode.Keypad3))
                        grid.tileColor = Color.green;
                    if (Event.current.keyCode == (KeyCode.Keypad4))
                        grid.tileColor = Color.blue;
                    if (Event.current.keyCode == (KeyCode.Keypad5))
                        grid.tileColor = Color.yellow;
                    if (Event.current.keyCode == (KeyCode.Keypad6))
                        grid.tileColor = Color.black;
                    if (Event.current.keyCode == (KeyCode.Keypad7))
                        grid.rotationZ += 90f;
                    if (Event.current.keyCode == (KeyCode.Keypad8))
                        grid.drag = !grid.drag;
                    if (Event.current.keyCode == (KeyCode.Keypad9))
                        grid.rotationZ -= 90f;

                    grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
                    grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
                    break;
                }
        }

        /* If nothing is pressed, lose control (not used)
        if (e.isMouse && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
        */

        if (grid.showPreview)
        {
            if (grid.mousePreview == null && GameObject.Find("Mouse Preview") == null)
            {
                grid.mousePreview = new GameObject("Mouse Preview");
                grid.mousePreview.AddComponent<SpriteRenderer>();
                grid.mousePreview.GetComponent<SpriteRenderer>().sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
                grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);

                grid.mousePreview.transform.localScale = grid.tilePrefab.transform.localScale;
                grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
            }
            else if (grid.mousePreview == null && GameObject.Find("Mouse Preview") != null)
            {
                DestroyImmediate(GameObject.Find("Mouse Preview"));
            }
            else
            {
                if (!grid.snapPreview)
                    grid.mousePreview.transform.position = mousePos;
                else
                {
                    grid.mousePreview.transform.position = new Vector3(
                        Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                        Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);
                }
            }
        }
        else if (grid.mousePreview != null)
            DestroyImmediate(grid.mousePreview);
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

            Vector3 aligned = Vector3.zero;
            if (grid.useGrid)
            {
                aligned = new Vector3(
                    Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                    Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);
            }
            else
            {
                aligned = mousePos;
            }


            //Debug.Log("Aligned: " + aligned);
            if (TileOnPosition(aligned) != -1 && !grid.overlap)
                return;

            if (grid.tiles == null)
            {
                grid.tiles = new GameObject("Tiles");
                grid.tiles.transform.parent = grid.transform;
                grid.tiles.hideFlags = HideFlags.HideInHierarchy;
            }

            spawnGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
            spawnGO.transform.position = new Vector2(aligned.x, aligned.y);
            spawnGO.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
            spawnGO.GetComponent<SpriteRenderer>().color = grid.tileColor;

            if (grid.hideInHierarchy)
                spawnGO.transform.parent = grid.tiles.transform;
            else
                spawnGO.transform.parent = grid.transform;

            grid.tileTransforms.Add(spawnGO.transform);

            Undo.RegisterCreatedObjectUndo(spawnGO, "Create " + spawnGO.name);
        }
    }

    private void RemoveTile(int controlID, Event e, Vector3 mousePos)
    {
        GUIUtility.hotControl = controlID;
        e.Use();

        Vector3 aligned = Vector3.zero;
        if (grid.useGrid)
        {
             aligned = new Vector3(
                Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);
        }
        else
        {
            aligned = mousePos;
        }

        //Debug.Log("Aligned: " + aligned);
        int tileOnPositionIndex = TileOnPosition(aligned);

        if (tileOnPositionIndex != -1)
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(grid.tileTransforms[tileOnPositionIndex].gameObject);
            grid.tileTransforms.RemoveAt(tileOnPositionIndex);
        }
    }

    private int TileOnPosition(Vector3 aligned)
    {
        if (grid.tileTransforms.Count <= 0)
            return -1;

        for (int i = 0; i < grid.tileTransforms.Count; i++)
        {
            if (grid.tileTransforms[i] == null)
            {
                grid.tileTransforms.RemoveAt(i);
                i--;
                continue;
            }

            Vector3 thisToCollider = new Vector3(grid.tileTransforms[i].position.x - aligned.x, grid.tileTransforms[i].position.y - aligned.y);

            float x = thisToCollider.x;
            float y = thisToCollider.y;
            float lengthSquared = x * x + y * y;

            float r = (grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.x + grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.y) / 4f;

            if (lengthSquared < r * r)
            {
                Debug.Log("i: " + i);
                return i;
            }
            /*
            Vector3 distance = new Vector3 (
                Mathf.Abs(grid.tileTransforms[i].position.x - aligned.x), 
                Mathf.Abs(grid.tileTransforms[i].position.y - aligned.y));

            if (distance.x < (grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.x / 2) && distance.y < (grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.x / 2))
                return i;
            */
        }
        return -1;
    }

}
