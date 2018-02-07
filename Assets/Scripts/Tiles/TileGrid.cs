using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public bool showGrid = true;
    public bool useGrid = true;
    public bool overlap = false;
    public bool drag = true;

    public bool mirror = true;
    public float mirrorOffset = 0f;
    public bool mirrorSprite = true;
    public bool removeMirrored = true;
    public bool useMirrored = false;
    public bool flipWorld = false;

    public float width = 1f;
    public float height = 1f;

    public Color color = Color.white;
    public Color tileColor = Color.white;

    public bool showPreview = true;
    public bool snapPreview = false;
    public float previewTransparency = 0.5f;


    public bool flipX = false;
    public bool hideInHierarchy = true;
    public GameObject tiles;
    public List<Transform> tileTransforms = new List<Transform>();

    public Transform tilePrefab;

    public TileSet tileSet;

    [HideInInspector]
    public bool debug = false;
    public Sprite sprite;
    public bool randomRotation = false;
    public float rotationZ = 0f;
    public GameObject mousePreview;

    public int tileIndex = 0;

    public int sortingOrder = 0;

    public bool hideFlag = true;

    public bool checkAdjacent = false;
    public bool restoreColliders = false;
    public bool resetAllSprites = false;
    public bool resetTransformList = false;

    private void OnDrawGizmos()
    {
        if (showGrid)
        {
            Vector3 pos = Camera.current.transform.position;
            Gizmos.color = color;

            // X Lines
            for (float y = pos.y - Screen.height; y < pos.y + Screen.height; y += height)
            {
                Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y / height) * height),
                                new Vector3(1000000.0f, Mathf.Floor(y / height) * height));
            }

            // Y Lines
            for (float x = pos.x - Screen.width; x < pos.x + Screen.width; x += width)
            {
                Gizmos.DrawLine(new Vector3(Mathf.Floor(x / width) * width, 1000000.0f),
                                new Vector3(Mathf.Floor(x / width) * width, -1000000.0f));
            }
        }
    }
}
