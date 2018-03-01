using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGrid))]  // Replace the inspector of TileGrid
public class GridEditor : Editor
{
    TileGrid grid;                  // The target script, a reference to the actual script

    private int oldIndex;           // Resets on enable, used to avoid errors with what tile is chosen (the tile index)

    private void OnEnable()        // Called when the inspector is opened (the GameObject is selected)
    {
        oldIndex = 0;               // Reset the index, this will be replaced either way
        grid =(TileGrid) target;   // Set grid to be a reference to the target
        SetTile();                 // Set the chosen tile to be selected, basically avoids problems with the tile being reset on each OnEnable()

        if (grid.tiles != null && grid.tileTransforms.Count == 0)               // If the list of transforms is empty, fill it with all of the tiles again
            for (int i = 0; i < grid.tiles.transform.childCount; i++)
                grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));

        /* 
        // Not used, will replace the list of transforms every time. Might be slow, therefore not used. There is a button for this in the inspector GUI

        grid.tileTransforms.Clear();
        for (int i = 0; i < grid.tiles.transform.childCount; i++)
            grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));
        */
    }

    // Enables right clicking in the Project tab to create a TileSet asset
    // This assets stores a list of tiles, it is selected in the inspector to sort the tiles into categories, or sets
    [MenuItem("Assets/Create/TileSet")]
    static void CreateTileSet()
    {
        TileSet asset = CreateInstance<TileSet>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(path))
            path = "Assets";
        else if (Path.GetExtension(path) != "")
            path = path.Replace(Path.GetFileName(path), "");
        else
            path += "/";

        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "TileSet.asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    // Called each time the Inspector is refreshed, so when a field is edited, changed or the GameObject is selected
    public override void OnInspectorGUI()
    {
        if (grid.tileSet == null)   // If there is no tileset assigned, null the selected tile prefab
            grid.tilePrefab = null;

        if (grid.tilePrefab != null && grid.tileSet != null)    // If a tile and tileset is assigned
        {
            grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;   // Assign the sprite preview field to be the same as the tile selected

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
            grid.useGrid = EditorGUILayout.Toggle("Use Grid", grid.useGrid);       // Toggle to use the grid or not
            if (EditorGUI.EndChangeCheck())
            {
                grid.snapPreview = grid.useGrid;    // Snap the preview to the grid, same as useGrid
                if (grid.useGrid)
                    grid.overlap = false;           // Disable overlap if the grid is used
            }

            grid.drag = EditorGUILayout.Toggle("Enable Drag", grid.drag);  // Clicking and dragging
            grid.overlap = EditorGUILayout.Toggle("Enable Overlap", grid.overlap); // Overlapping, disables detecting if a block is already placed on the position

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mirror Settings", EditorStyles.boldLabel);
            grid.mirror = EditorGUILayout.Toggle(new GUIContent("Mirror", "Enable mirroring on the Y axis."), grid.mirror);

            EditorGUILayout.Space();

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
                grid.useMirrored = EditorGUILayout.Toggle(new GUIContent("Use Mirrored", "Place the blocks for the mirrored world with normal orientation."), grid.useMirrored);
                grid.flipWorld = EditorGUILayout.Toggle(new GUIContent("Flip World", "Place the dark world on top."), grid.flipWorld);
            }
            if (grid.flipWorld && !grid.mirror)     // Flip the world
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

        // Tileset
        EditorGUI.BeginChangeCheck();
        TileSet newTileSet =(TileSet) EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false); // Tileset field
        if (EditorGUI.EndChangeCheck())
        {
            grid.tileSet = newTileSet;
            SetTile();
        }

        if (grid.tileSet != null)   // If a tileset is selected
        {
            EditorGUI.BeginChangeCheck();
            string[] names = new string[grid.tileSet.prefabs.Length];   // Store the names
            int[] values = new int[names.Length];                       // and values in arrays

            for (int i = 0; i < names.Length; i++)                      // For each name index
            {
                names[i] = grid.tileSet.prefabs[i] != null ? grid.tileSet.prefabs[i].name : ""; // Store the name of the tile prefab
                values[i] = i;                                                                  // Store the index i in the values array at index i
            }

            grid.sortingOrder = EditorGUILayout.IntField("Sorting Order:", grid.sortingOrder);  // Sorting Order field
            grid.tileIndex = EditorGUILayout.IntPopup("Select Tile", oldIndex, names, values);  // Int Popup with the tiles

            if (EditorGUI.EndChangeCheck())
                if (oldIndex != grid.tileIndex) // If a new tile is selected
                    SetTile();                  // SetTile() to select it


            if (grid.sprite != null)    // If there is a sprite, display it as a preview
                EditorGUILayout.ObjectField(new GUIContent("Tile Sprite", "[READ-ONLY] Changing this does not affect anything."), grid.sprite, typeof(Sprite), false);
        }

        EditorGUI.BeginChangeCheck();
        grid.hideFlag = EditorGUILayout.Toggle(new GUIContent("Hideflag on parent", "Hide or show the parent object in the hierarchy."), grid.hideFlag);
        if (EditorGUI.EndChangeCheck())
            if (grid.hideFlag)
                grid.tiles.hideFlags = HideFlags.HideInHierarchy;
            else
                grid.tiles.hideFlags = HideFlags.None;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Function Buttons", EditorStyles.boldLabel); // Used as buttons for several functions
        EditorGUI.BeginChangeCheck();
        grid.checkAdjacent = EditorGUILayout.Toggle(new GUIContent("Check Adjacent", "[CAN'T UNDO] Calls the function to disabe colliders on occluded objects."), grid.checkAdjacent);
        if (EditorGUI.EndChangeCheck())
            CheckAdjacentBlocks();

        EditorGUI.BeginChangeCheck();
        grid.restoreColliders = EditorGUILayout.Toggle(new GUIContent("Restore Colliders", "[CAN'T UNDO] Restores the colliders on all tiles."), grid.restoreColliders);
        if (EditorGUI.EndChangeCheck())
            RestoreColliders();

        EditorGUI.BeginChangeCheck();
        grid.resetAllSprites = EditorGUILayout.Toggle(new GUIContent("Reset All Sprites", "[CAN'T UNDO] Uses the position to set the appropriate sprite."), grid.resetAllSprites);
        if (EditorGUI.EndChangeCheck())
            ResetAllSprites();

        EditorGUI.BeginChangeCheck();
        grid.resetTransformList = EditorGUILayout.Toggle(new GUIContent("Reset Transform List", "[CAN'T UNDO] Clear and replace the entire transform tile list."), grid.resetTransformList);
        if (EditorGUI.EndChangeCheck())
            ResetTransformList();

        EditorGUI.BeginChangeCheck();
        grid.addAsChildren = EditorGUILayout.Toggle(new GUIContent("Add all tiles as children", "[CAN'T UNDO] Clear and replace the entire transform tile list."), grid.addAsChildren);
        if (EditorGUI.EndChangeCheck())
            AddTilesAsChildren();

        EditorGUI.BeginChangeCheck();
        grid.SetScaleTo1 = EditorGUILayout.Toggle(new GUIContent("Set scale to 1.01", "[CAN'T UNDO] Takes all gameobjects with a scale value of 1 and makes it 1.01"), grid.SetScaleTo1);
        if (EditorGUI.EndChangeCheck())
            SetScaleTo1();

        EditorGUI.BeginChangeCheck();
        grid.selectTilesWithName = EditorGUILayout.Toggle(new GUIContent("Select all with name same as prefab", ""), grid.selectTilesWithName);
        if (EditorGUI.EndChangeCheck())
            SelectTilesWithName(grid.tileSet.prefabs[grid.tileIndex].name);

        EditorGUILayout.Space();
        grid.debug = EditorGUILayout.Toggle(new GUIContent("Display Debug", "Displays debug variables. Useful for debugging."), grid.debug);

        if (grid.debug)
            base.OnInspectorGUI();
    }

    private void SetTile()
    {
        if (grid.tileSet == null)
            return;

        oldIndex = grid.tileIndex;  // Assign the index
        if (grid.tileIndex < grid.tileSet.prefabs.Length)                       // Prevent array out-of-range
            grid.tilePrefab = grid.tileSet.prefabs[grid.tileIndex];             // Select the tile at the current tileIndex
        grid.sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;    // Fetch the sprite

        if (grid.tilePrefab.GetComponent<Renderer>().bounds.size.x != 1.01f)
            grid.width = grid.tilePrefab.GetComponent<Renderer>().bounds.size.x;    // Set grid width to the size of the tile
        else
            grid.width = 1f;
        if (grid.tilePrefab.GetComponent<Renderer>().bounds.size.y != 1.01f)
            grid.height = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;   // Same with height
        else
            grid.height = 1f;

        if (grid.mousePreview != null)                                          // If there is a mousepreview
        {
            grid.mousePreview.GetComponent<SpriteRenderer>().sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;            // Assign the same sprite as the tile
            grid.mousePreview.transform.localScale = grid.tilePrefab.transform.localScale;                                              // Set the scale same as the tile
            grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);  // Get the color of the tile
        }
    }

    // Function called from a button
    // Used for disabling surrounded colliders
    private void CheckAdjacentBlocks()
    {
        RestoreColliders();                                                                 // Restore the colliders, they are needed for the checking
        List<Transform> chosen = new List<Transform>();
        for (int i = 0; i < grid.tileTransforms.Count; i++)                                 // For each tile transform
        {
            if (grid.tileTransforms[i].GetComponent<CheckAdjacent>() != null)               // If they have the CheckAdjacent script,
                if (grid.tileTransforms[i].GetComponent<CheckAdjacent>().DoCheck(i) == -1)  // and return that they are surrounded,
                    chosen.Add(grid.tileTransforms[i]);                                     // add them to the list
        }
        foreach (Transform tile in chosen)                                                  // For each chosen transform,
        {
            BoxCollider2D[] colliders = tile.GetComponents<BoxCollider2D>();                // get the box colliders
            foreach (BoxCollider2D collider in colliders)                                   // For each box collider,
            {
                if (collider.enabled)                                                       // if they are enabled,
                {
                    collider.enabled = false;                                               // disable them
                    Debug.Log("Disabled colliders");
                }
            }
            PlatformEffector2D[] effectors = tile.GetComponents<PlatformEffector2D>();
            foreach(PlatformEffector2D e in effectors)
            {
                if(e.enabled)
                {
                    e.enabled = false;
                    Debug.Log("Disabled effectors");
                }
            }
        }
        Debug.Log("Checked all objects");
        grid.checkAdjacent = false;                                                         // Set the bool to false to prevent being checked in the inspector
    }

    // Same function as CheckAdjacentBlocks(), but only affects the selected objects
    // Called from the Tools menu
    [MenuItem("4K Studios Tools/Grid/Check adjacent blocks on selected")]
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
            BoxCollider2D[] colliders = tile.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D collider in colliders)
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
        foreach (Transform tile in grid.tileTransforms)                         // For each tile transform,
        {
            BoxCollider2D[] colliders = tile.GetComponents<BoxCollider2D>();    // get the box colliders
            foreach (BoxCollider2D collider in colliders)                       // For each box collider,
            {
                if (!collider.enabled)                                          // if they are disabled,
                {
                    collider.enabled = true;                                    // enable them
                    Debug.Log("Enabled colliders");
                }
            }
        }
        grid.restoreColliders = false;
    }

    [MenuItem("4K Studios Tools/Grid/Restore Colliders on selected")]
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

    void AddTilesAsChildren()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int amount = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.tag == "Floor" ||
                obj.GetComponent<CombinedTile>() != null ||
                obj.GetComponent<PlatformEffector2D>() != null ||
                obj.GetComponent<CheckAdjacent>() != null)
            {
                obj.transform.parent = grid.tiles.transform;
                amount++;
            }
        }
        Debug.Log(amount + " tiles added to Tiles as children.");
        grid.addAsChildren = false;
    }

    private void ResetAllSprites()
    {
        foreach (Transform tile in grid.tileTransforms)                                                     // For each tile transform,
        {
            if (tile.GetComponent<SpriteRenderer>() != null && tile.GetComponent<CombinedTile>() != null)   // if they have a SpriteRenderer and CombinedTile,
                if (tile.position.y < grid.mirrorOffset)
                    tile.GetComponent<CombinedTile>().SetTileLight(false);                                  // set to the dark version if under 0,
                else
                    tile.GetComponent<CombinedTile>().SetTileLight(true);                                   // or the light version if over 0
        }
        grid.resetAllSprites = false;
    }

    private void ResetTransformList()
    {
        grid.tileTransforms.Clear();                                    // Clear the transform list
        for (int i = 0; i < grid.tiles.transform.childCount; i++)       // For each child under "tiles",
            grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));  // add them to the list

        grid.resetTransformList = false;
    }

    private void SetScaleTo1()
    {
        foreach(Transform t in grid.tileTransforms)
        {
            if (Mathf.Abs(t.localScale.x) == 1f)
                t.localScale = new Vector3(1.01f * t.localScale.x, t.localScale.y, t.localScale.z);
            if (Mathf.Abs(t.localScale.y) == 1f)
                t.localScale = new Vector3(t.localScale.x, 1.01f * t.localScale.y, t.localScale.z);

            t.localScale = new Vector3(t.localScale.x, t.localScale.y, 1.0f);
        }
        grid.SetScaleTo1 = false;
    }

    private void SelectTilesWithName(string name)
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<GameObject> gos = new List<GameObject>();

        foreach(GameObject go in gameObjects)
        {
            if(go.name == name)
                gos.Add(go);
        }
        Selection.objects = gos.ToArray();
        grid.selectTilesWithName = false;
    }

    // Prevents a float field from being smaller than the min value
    // Prevents the grideditor from crashing Unity when dividing by 0
    private float MinFloat(string labelName, float value, float min)
    {
        value = EditorGUILayout.FloatField(labelName, value);
        if (value < min)
            value = min;
        return value;
    }

    // Method for creating a slider correctly
    private float CreateSlider(string labelName, float sliderPosition, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(labelName);
        sliderPosition = EditorGUILayout.Slider(sliderPosition, min, max);
        GUILayout.EndHorizontal();

        return sliderPosition;
    }

    // Called when input is given to the Scene tab
    private void OnSceneGUI()
    {
        if (grid.tilePrefab == null)    // If no tile is selected, return
            return;

        int controlID = GUIUtility.GetControlID(FocusType.Passive); // Take control of the window
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePos = ray.origin;

        if (grid.drag)
        {
            // Left click and drag to place prefab
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 || e.isMouse && e.type == EventType.MouseDrag && e.button == 0)
                PlaceTile(controlID, e, ray, mousePos);

            // Right click and drag to remove prefab
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

        // Keyboard Input
        switch (e.type)
        {
            case EventType.KeyDown:
                {
                    if (Event.current.keyCode == (KeyCode.Keypad1))             // Set colors with 1, 2, 3, 4, 5 and 6
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
                    if (Event.current.keyCode == (KeyCode.Keypad7))             // Rotate 90 degrees with 7 and 9
                        grid.rotationZ += 90f;
                    if (Event.current.keyCode == (KeyCode.Keypad8))             // Enable/Disable drag with 8
                        grid.drag = !grid.drag;
                    if (Event.current.keyCode == (KeyCode.Keypad9))
                        grid.rotationZ -= 90f;
                    if (Event.current.keyCode == (KeyCode.KeypadPlus))          // Switch tile with +/-
                        if (grid.tileIndex < grid.tileSet.prefabs.Length - 1)
                            grid.tileIndex++;
                        else
                            grid.tileIndex = 0;
                    if (Event.current.keyCode == (KeyCode.KeypadMinus))
                        if (grid.tileIndex > 0)
                            grid.tileIndex--;
                        else
                            grid.tileIndex = grid.tileSet.prefabs.Length - 1;

                    SetTile();  // Update the tile

                    // Apply changes to the mouse preview
                    grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
                    grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);
                    if (grid.flipX)
                        grid.mousePreview.transform.localScale = new Vector2(-Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
                    else
                        grid.mousePreview.transform.localScale = new Vector2(Mathf.Abs(grid.mousePreview.transform.localScale.x), grid.mousePreview.transform.localScale.y);
                    break;
                }
        }

        ShowPreview(mousePos);

        // If a redo or undo has happened
        if(grid.didUndo)
        {
            // Reset the transform list
            grid.tileTransforms.Clear();
            for (int i = 0; i < grid.tiles.transform.childCount; i++)
                grid.tileTransforms.Add(grid.tiles.transform.GetChild(i));

            grid.didUndo = false;
        }
    }

    private void ShowPreview(Vector3 mousePos)
    {
        if (grid.showPreview && grid.tilePrefab != null)
        {
            if (grid.mousePreview == null && GameObject.Find("Mouse Preview") == null)  // If there is no mouse preview
            {
                // Create a mouse preview gameobject with the correct components
                grid.mousePreview = new GameObject("Mouse Preview");
                grid.mousePreview.AddComponent<SpriteRenderer>();
                grid.mousePreview.AddComponent<CombinedTile>();
                grid.mousePreview.GetComponent<SpriteRenderer>().sprite = grid.tilePrefab.GetComponent<SpriteRenderer>().sprite;
                grid.mousePreview.GetComponent<SpriteRenderer>().color = grid.tileColor - new Color(0f, 0f, 0f, grid.previewTransparency);

                grid.mousePreview.transform.localScale = grid.tilePrefab.transform.localScale;
                grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);
            }
            else if (grid.mousePreview == null && GameObject.Find("Mouse Preview") != null) // If it is not assigned, but exists: Destroy it
            {
                DestroyImmediate(GameObject.Find("Mouse Preview"));
            }
            else    // If there is a mouse preview
            {
                if (!grid.snapPreview)                                                                  // Set to mouse position if not snapping
                    grid.mousePreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
                else                                                                                    // Otherwise, snap
                {
                    grid.mousePreview.transform.position = new Vector3(
                        Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                        Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f, 0f);
                }

                if (grid.mirror)                                                                            // If mirroring
                {
                    grid.mousePreview.GetComponent<CombinedTile>().FlipY(mousePos.y < grid.mirrorOffset);   // Flip the mouse preview tile if under 0

                    if (grid.mirrorSprite)                                                                                          // If the sprite is to be switched on the mirror side
                    {
                        if (grid.tilePrefab.GetComponent<CombinedTile>() != null)                                                   // If the tile has the CombinedTile script
                        {
                            grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>()); // Get the values from the tile and assign them to the preview
                            if (mousePos.y < grid.mirrorOffset)
                                grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(false);                                 // Set to dark if under 0,
                            else
                                grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(true);                                  // or to light if over 0
                        }
                    }
                }
                else if (grid.useMirrored)  // Use mirrored version without mirroring
                {
                    if (grid.tilePrefab.GetComponent<CombinedTile>() != null)
                    {
                        grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>());
                        grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(false); // Set to the dark version
                    }
                }
                else
                {
                    if (grid.tilePrefab.GetComponent<CombinedTile>() != null)
                    {
                        grid.mousePreview.GetComponent<CombinedTile>().SetSameAs(grid.tilePrefab.GetComponent<CombinedTile>());
                        grid.mousePreview.GetComponent<CombinedTile>().SetTileLight(true);  // If not mirroring, set to the light version
                    }
                }
            }
        }
        else if (grid.mousePreview != null) // If there is not supposed to be a preview, destroy it
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
            if (grid.useGrid)   // If use grid, align to grid. Otherwise use mousePos
            {
                aligned = new Vector3(
                    Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                    Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f);
            }
            else
                aligned = mousePos;

            if (grid.mirror)    // If mirroring, store the mirrored vector of aligned
                mirrored = new Vector2(aligned.x, -aligned.y + grid.mirrorOffset * 2f);

            if (grid.tiles == null) // If there is no parent object for the tiles
            {
                grid.tiles = new GameObject("Tiles");               // Create a gameobject to use as parent
                grid.tiles.transform.parent = grid.transform;       // Set it to be a child of the editor object
                grid.tiles.hideFlags = HideFlags.HideInHierarchy;   // If enabled, hide it in the hierarchy
            }

            if (grid.randomRotation)                                                                // If random rotation is enabled
            {
                grid.rotationZ = Random.Range(0, 360);                                              // Randomize a degree between 0 and 360
                grid.mousePreview.transform.rotation = Quaternion.Euler(0f, 0f, grid.rotationZ);    // Use it on the preview
            }

            if (TileOnPosition(aligned) == -1 || grid.overlap)                                      // If there is no object on the "aligned" position, or overlap is enabled
            {

                spawnGO =(GameObject) PrefabUtility.InstantiatePrefab(prefab.gameObject);
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
                mirrorGO =(GameObject) PrefabUtility.InstantiatePrefab(prefab.gameObject);
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
                            if (spawnGO != null)
                                spawnGO.GetComponent<CombinedTile>().SetTileLight(true);
                        }
                        else
                        {
                            mirrorGO.GetComponent<CombinedTile>().SetTileLight(true);
                            if (spawnGO != null)
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
                    if (spawnGO != null)
                        spawnGO.GetComponent<CombinedTile>().FlipY(true);
                    if (mirrorGO != null)
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
            else if (TileOnPosition(mirrored) == -1 || grid.overlap)
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

        if(tileOnPositionIndex != -1)
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(grid.tileTransforms[tileOnPositionIndex].gameObject);
            grid.tileTransforms.RemoveAt(tileOnPositionIndex);
        }

        if (grid.mirror && grid.removeMirrored)
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

            float r =(grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.x + grid.tileTransforms[i].GetComponent<Renderer>().bounds.size.y) / 4f;

            if (lengthSquared < r * r)
                return i;
        }
        return -1;
    }
}