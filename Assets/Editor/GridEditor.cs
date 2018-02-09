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
        SetTile();

        if (grid.tiles != null && grid.tileTransforms.Count == 0)
            for(int i = 0; i < grid.tiles.transform.childCount; i++)
                grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));
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
        if (grid.tileSet == null)
            grid.tilePrefab = null;

        if (grid.tilePrefab != null && grid.tileSet != null)
        {
            grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
            grid.useGrid = EditorGUILayout.Toggle("Use Grid", grid.useGrid);
            if (EditorGUI.EndChangeCheck())
            {
                grid.snapPreview = grid.useGrid;
                if (grid.useGrid)
                    grid.overlap = false;
            }

            grid.drag = EditorGUILayout.Toggle("Enable Drag", grid.drag);
            grid.overlap = EditorGUILayout.Toggle("Enable Overlap", grid.overlap);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mirror Settings", EditorStyles.boldLabel);
            grid.mirror = EditorGUILayout.Toggle(new GUIContent("Mirror", "Enable mirroring on the Y axis."), grid.mirror);
            if (grid.mirror)
            {
                grid.mirrorOffset = EditorGUILayout.FloatField(new GUIContent("Mirror Offset", "Offsets the Y axis. Basically raises and lowers the mirroring point."), grid.mirrorOffset);
                grid.mirrorSprite = EditorGUILayout.Toggle(new GUIContent("Mirror Sprites", "Enables the sprites of the mirrored object to be automatically switched."), grid.mirrorSprite);
                grid.removeMirrored = EditorGUILayout.Toggle(new GUIContent("Remove Mirror", "Remove tiles on both sides."), grid.removeMirrored);
                grid.useMirrored = false;
                grid.flipWorld = false;
            }
            else
            {
                EditorGUILayout.Space();
                grid.useMirrored = EditorGUILayout.Toggle(new GUIContent("Use Mirrored", "Place the blocks for the mirrored world with normal orientation."), grid.useMirrored);
                grid.flipWorld = EditorGUILayout.Toggle(new GUIContent("Flip World", "Place the dark world on top."), grid.flipWorld);
            }
            if (grid.flipWorld && !grid.mirror)
                grid.transform.localScale = new Vector2(1f, -1f);
            else
                grid.transform.localScale = new Vector2(1f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            grid.showGrid = EditorGUILayout.Toggle("Show Grid", grid.showGrid);
            grid.width = MinFloat("Width", grid.width, 0.01f);
            grid.height = MinFloat("Height", grid.height, 0.01f);

            EditorGUI.BeginChangeCheck();
            Color newColor = EditorGUILayout.ColorField("Grid Color", grid.color);
            if (EditorGUI.EndChangeCheck())
                grid.color = newColor;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mouse Preview", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            grid.showPreview = EditorGUILayout.Toggle("Show Preview", grid.showPreview);
            if (grid.showPreview)
            {
                grid.snapPreview = EditorGUILayout.Toggle("Snap to Grid", grid.snapPreview);
                grid.previewTransparency = CreateSlider("Preview Transparency", grid.previewTransparency, 0f, 1f);
            }
            else
                grid.snapPreview = false;

            if (EditorGUI.EndChangeCheck())

                if (grid.mousePreview != null)
                    grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);

            EditorGUILayout.Space();
        }
        EditorGUILayout.LabelField("Tile Settings", EditorStyles.boldLabel);
        grid.tileColor = EditorGUILayout.ColorField("Tile Color:", grid.tileColor);

        EditorGUI.BeginChangeCheck();
        grid.flipX = EditorGUILayout.Toggle(new GUIContent("Flip X", "Flips the scale on the X axis, not the sprite"), grid.flipX);
        grid.randomRotation = EditorGUILayout.Toggle(new GUIContent("Random Rotation", "Enables random rotation on each click"), grid.randomRotation);
        grid.rotationZ = EditorGUILayout.FloatField("Rotaton", grid.rotationZ);

        if (EditorGUI.EndChangeCheck())
        {
            grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
            if (grid.flipX)
                grid.mousePreview.transform.localScale = new Vector2(-Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
            else
                grid.mousePreview.transform.localScale = new Vector2(Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
        }

        grid.hideInHierarchy = EditorGUILayout.Toggle(new GUIContent("Hide in Hierarchy", "Placed tiles will be invisible in the Hierarchy window. They are still visible in the debug variables."), grid.hideInHierarchy);

        /*
        // Tile Prefab
        /*
        EditorGUI.BeginChangeCheck();
        Transform newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab", grid.tilePrefab, typeof(Transform), false);
        if(EditorGUI.EndChangeCheck())
        {
            grid.tilePrefab = newTilePrefab;
        }
        */

        // Tile Map
        EditorGUI.BeginChangeCheck();
        TileSet newTileSet = (TileSet)EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false);
        if(EditorGUI.EndChangeCheck())
        {
            grid.tileSet = newTileSet;
            SetTile();
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

            grid.sortingOrder = EditorGUILayout.IntField("Sorting Order:", grid.sortingOrder);
            grid.tileIndex = EditorGUILayout.IntPopup("Select Tile", oldIndex,names,values);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Grid Changed");
                if(oldIndex != grid.tileIndex)
                {
                    SetTile();
                }
            }
            if(grid.sprite != null)
                EditorGUILayout.ObjectField(new GUIContent("Tile Sprite", "READ-ONLY. Changing this does not affect anything."), grid.sprite, typeof(Sprite), false);
        }

        EditorGUI.BeginChangeCheck();
        grid.hideFlag = EditorGUILayout.Toggle(new GUIContent("Hideflag on parent", "Hide or show the parent object in the hierarchy."), grid.hideFlag);
        if(EditorGUI.EndChangeCheck())
            if(grid.hideFlag)
                grid.tiles.hideFlags = HideFlags.HideInHierarchy;
            else
                grid.tiles.hideFlags = HideFlags.None;
        
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        grid.checkAdjacent = EditorGUILayout.Toggle(new GUIContent("Check Adjacent", "Calls the function to disabe colliders on occluded objects."), grid.checkAdjacent);
        if(EditorGUI.EndChangeCheck())
            CheckAdjacentBlocks();

        EditorGUI.BeginChangeCheck();
        grid.restoreColliders = EditorGUILayout.Toggle(new GUIContent("Restore Colliders", "Restores the colliders on all tiles."), grid.restoreColliders);
        if (EditorGUI.EndChangeCheck())
            RestoreColliders();

        EditorGUI.BeginChangeCheck();
        grid.resetAllSprites = EditorGUILayout.Toggle(new GUIContent("Reset All Sprites", "Uses the position to set the appropriate sprite."), grid.resetAllSprites);
        if (EditorGUI.EndChangeCheck())
            ResetAllSprites();

        EditorGUI.BeginChangeCheck();
        grid.resetTransformList = EditorGUILayout.Toggle(new GUIContent("Reset Transform List", "Clear and replace the entire transform tile list."), grid.resetTransformList);
        if (EditorGUI.EndChangeCheck())
            ResetTransformList();

        EditorGUILayout.Space();
        grid.debug = EditorGUILayout.Toggle(new GUIContent("Display Debug", "Displays debug variables. Useful for debugging."), grid.debug);

        if(grid.tilePrefab != null)
            grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;

        if (grid.debug)
            base.OnInspectorGUI();
    }

    private void SetTile()
    {
        if (grid.tileSet == null)
            return;

        oldIndex = grid.tileIndex;
        if(grid.tileIndex < grid.tileSet.prefabs.Length)
            grid.tilePrefab = grid.tileSet.prefabs[grid.tileIndex];
        grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
        //grid.rotationZ = 0f;

        float width = grid.tilePrefab.GetComponent<Renderer>().bounds.size.x;
        float height = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;

        grid.width = width;
        grid.height = height;

        if (grid.mousePreview != null)
        {
            grid.mousePreview.GetComponent<SpriteRenderer>().sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
            grid.mousePreview.transform.localScale = grid.tilePrefab.transform.localScale;
            grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
        }
    }

    private void CheckAdjacentBlocks() 
    {
        List<Transform> chosen = new List<Transform>();
        for (int i = 0; i < grid.tileTransforms.Count; i++)
        {
            if (grid.tileTransforms[i].GetComponent<CheckAdjacent>() != null)
            {
                if(grid.tileTransforms[i].GetComponent<CheckAdjacent>().DoCheck(i) == -1) 
                {
                    chosen.Add(grid.tileTransforms[i]);
                }
            }
        }
        foreach(Transform tile in chosen) 
        {
            Collider2D[] colliders = tile.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (collider.enabled)
                {
                    collider.enabled = false;
                    Debug.Log("Disabled colliders");
                }
            }
        }
        Debug.Log("Checked all objects");
        grid.checkAdjacent = false;
    }

    [MenuItem("Tools/Grid/Check adjacent blocks on selected")]
    static void CheckAdjacentBlocksSelected()
    {
        List<Transform> chosen = new List<Transform>();
        Transform[] selection = Selection.transforms;
        for (int i = 0; i < selection.Length; i++)
        {
            if (selection[i].GetComponent<CheckAdjacent>() != null)
            {
                if (selection[i].GetComponent<CheckAdjacent>().DoCheck(i) == -1)
                {
                    chosen.Add(selection[i]);
                }
            }
        }
        foreach (Transform tile in chosen)
        {
            Collider2D[] colliders = tile.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (collider.enabled)
                {
                    collider.enabled = false;
                    Debug.Log("Disabled colliders");
                }
            }
        }
        Debug.Log("Checked selected");
    }

    private void RestoreColliders() 
    {
        foreach(Transform tile in grid.tileTransforms)
        {
            Collider2D[] colliders = tile.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled)
                {
                    collider.enabled = true;
                    Debug.Log("Enabled colliders");
                }
            }
        }
        grid.restoreColliders = false;
    }

    [MenuItem("Tools/Grid/Restore Colliders on selected")]
    static void RestoreCollidersSelected()
    {
        Transform[] selection = Selection.transforms;
        foreach (Transform tile in selection)
        {
            Collider2D[] colliders = tile.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled)
                {
                    collider.enabled = true;
                    Debug.Log("Enabled colliders");
                }
            }
        }
    }

    private void ResetAllSprites() 
    {
        foreach (Transform tile in grid.tileTransforms)
        {
            if(tile.GetComponent<SpriteRenderer>() != null && tile.GetComponent<CombinedTile>() != null)
            if (tile.position.y < grid.mirrorOffset)
                tile.GetComponent<CombinedTile>().SetTileLight(false);
            else
                tile.GetComponent<CombinedTile>().SetTileLight(true);
        }
        grid.resetAllSprites = false;
    }

    private void ResetTransformList() 
    {
        grid.tileTransforms.Clear();
        for (int i = 0; i < grid.tiles.transform.childCount; i++)
            grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));

        grid.resetTransformList = false;
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
        if (grid.tilePrefab == null)
            return;

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
                    if (Event.current.keyCode == (KeyCode.KeypadPlus))
                        if (grid.tileIndex < grid.tileSet.prefabs.Length - 1)
                            grid.tileIndex++;
                        else
                            grid.tileIndex = 0;
                    if (Event.current.keyCode == (KeyCode.KeypadMinus))
                        if (grid.tileIndex > 0)
                            grid.tileIndex--;
                        else
                            grid.tileIndex = grid.tileSet.prefabs.Length - 1;

                    SetTile();
                    grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
                    grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
                    if (grid.flipX)
                        grid.mousePreview.transform.localScale = new Vector2(-Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
                    else
                        grid.mousePreview.transform.localScale = new Vector2(Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
                    break;
                }
        }

        /* If nothing is pressed, lose control (not used)
        if (e.isMouse && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
        */

        ShowPreview(mousePos);
    }

    private void ShowPreview(Vector3 mousePos)
    {
        if (grid.showPreview && grid.tilePrefab != null)
        {
            if (grid.mousePreview == null && GameObject.Find("Mouse Preview") == null)
            {
                grid.mousePreview = new GameObject("Mouse Preview");
                grid.mousePreview.AddComponent<SpriteRenderer>();
                grid.mousePreview.AddComponent<CombinedTile>();
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
                    grid.mousePreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
                else
                {
                    grid.mousePreview.transform.position = new Vector3(
                        Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                        Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f
                        , 0f);
                }

                if (grid.mirror)
                {
                    grid.mousePreview.GetComponent<CombinedTile>().FlipY(mousePos.y < grid.mirrorOffset);

                    if (grid.mirrorSprite)
                    {
                        if (grid.tilePrefab.GetComponent<CombinedTile>() != null)
                        {
                            grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>());
                            if (mousePos.y < grid.mirrorOffset)
                                grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(false);
                            else
                                grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(true);
                        }
                    }
                }
                else if (grid.useMirrored)
                {
                    if (grid.tilePrefab.GetComponent<CombinedTile>() != null)
                    {
                        grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>());
                        grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(false);
                    }
                }
                else
                {
                    if (grid.tilePrefab.GetComponent<CombinedTile>() != null)
                    {
                        grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>());
                        grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(true);
                    }
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

        GameObject spawnGO = null;
        GameObject mirrorGO = null;
        Transform prefab = grid.tilePrefab;

        if (prefab)
        {
            Undo.IncrementCurrentGroup();

            Vector3 aligned = Vector3.zero;
            Vector3 mirrored = Vector3.zero;
            if (grid.useGrid)
            {
                aligned = new Vector3(
                    Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                    Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);
            }
            else
                aligned = mousePos;

            if(grid.mirror)
                mirrored = new Vector2(aligned.x, -aligned.y + grid.mirrorOffset * 2f);

            if (grid.tiles == null)
            {
                grid.tiles = new GameObject("Tiles");
                grid.tiles.transform.parent = grid.transform;
                grid.tiles.hideFlags = HideFlags.HideInHierarchy;
            }

            if (grid.randomRotation)
            {
                grid.rotationZ = Random.Range(0, 360);
                grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
            }

            if (TileOnPosition(aligned) == -1 || grid.overlap)
            {

                spawnGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
                spawnGO.transform.position = new Vector2(aligned.x, aligned.y);
                spawnGO.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
                spawnGO.GetComponent<SpriteRenderer>().color = grid.tileColor;
                spawnGO.GetComponent<SpriteRenderer>().sortingOrder = grid.sortingOrder;

                if (grid.flipX)
                    spawnGO.transform.localScale = new Vector2(-Mathf.Abs(spawnGO.transform.localScale.x), spawnGO.transform.localScale.y);

                if (grid.hideInHierarchy)
                    spawnGO.transform.parent = grid.tiles.transform;
                else
                    spawnGO.transform.parent = grid.transform;

                grid.tileTransforms.Add(spawnGO.transform);

                Undo.RegisterCreatedObjectUndo(spawnGO, "Create " + spawnGO.name);

            }

            if (grid.mirror && TileOnPosition(mirrored) == -1 || grid.overlap)
            {
                mirrorGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
                mirrorGO.transform.position = new Vector2(mirrored.x, mirrored.y);
                mirrorGO.transform.rotation = Quaternion.Euler(0f, 0f, -grid.rotationZ);
                mirrorGO.GetComponent<SpriteRenderer>().sortingOrder = grid.sortingOrder;

                if (grid.flipX)
                    mirrorGO.transform.localScale = new Vector2(-Mathf.Abs(mirrorGO.transform.localScale.x), mirrorGO.transform.localScale.y);

                if (grid.mirrorSprite)
                {
                    if (mirrorGO.GetComponent<CombinedTile>() != null)
                    {
                        if (aligned.y > mirrored.y)
                        {
                            mirrorGO.GetComponent<CombinedTile>().SetTileLight(false);
                            if(spawnGO != null)
                                spawnGO.GetComponent<CombinedTile>().SetTileLight(true);
                        }
                        else
                        {
                            mirrorGO.GetComponent<CombinedTile>().SetTileLight(true);
                            if(spawnGO != null)
                                spawnGO.GetComponent<CombinedTile>().SetTileLight(false);
                        }
                    }
                }

                mirrorGO.GetComponent<SpriteRenderer>().color = grid.tileColor;

                if (grid.hideInHierarchy)
                    mirrorGO.transform.parent = grid.tiles.transform;
                else
                    mirrorGO.transform.parent = grid.transform;

                grid.tileTransforms.Add(mirrorGO.transform);

                if (mousePos.y < grid.mirrorOffset)
                {
                    if(spawnGO != null)
                        spawnGO.GetComponent<CombinedTile>().FlipY(true);
                    if(mirrorGO != null)
                        mirrorGO.GetComponent<CombinedTile>().FlipY(false);
                }
                else
                {
                    if (spawnGO != null)
                        spawnGO.GetComponent<CombinedTile>().FlipY(false);
                    if (mirrorGO != null)
                        mirrorGO.GetComponent<CombinedTile>().FlipY(true);
                }
            }
            else if (grid.useMirrored && TileOnPosition(mirrored) == -1 || grid.overlap)
            {
                if (spawnGO != null && spawnGO.GetComponent<CombinedTile>() != null)
                    spawnGO.GetComponent<CombinedTile>().SetTileLight(false);
            }
            else if(TileOnPosition(mirrored) == -1 || grid.overlap)
            { 
                if (spawnGO != null && spawnGO.GetComponent<CombinedTile>() != null)
                    spawnGO.GetComponent<CombinedTile>().SetTileLight(true);
            }

            if (grid.mirror && mirrorGO != null)
                Undo.RegisterCreatedObjectUndo(mirrorGO, "Mirror " + mirrorGO.name);
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
            aligned = mousePos;

        int tileOnPositionIndex = TileOnPosition(aligned);

        if (tileOnPositionIndex != -1)
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(grid.tileTransforms[tileOnPositionIndex].gameObject);
            grid.tileTransforms.RemoveAt(tileOnPositionIndex);
        }

        if(grid.mirror && grid.removeMirrored)
        {
            tileOnPositionIndex = TileOnPosition(new Vector2(aligned.x, -aligned.y + grid.mirrorOffset * 2f));

            if (tileOnPositionIndex != -1)
            {
                Undo.IncrementCurrentGroup();
                Undo.DestroyObjectImmediate(grid.tileTransforms[tileOnPositionIndex].gameObject);
                grid.tileTransforms.RemoveAt(tileOnPositionIndex);
            }
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
                return i;
        }
        return -1;
    }

}
