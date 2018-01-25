using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public float width = 1f;
    public float height = 1f;

    public Color color = Color.white;
    public Color tileColor = Color.white;

    public bool showPreview = true;
    public bool snapPreview = false;
    public float previewTransparency = 0.5f;

    public bool hideInHierarchy = true;
    public GameObject tiles;
    public List<Transform> tileTransforms = new List<Transform>();

    public Transform tilePrefab;

    public TileSet tileSet;

    [HideInInspector]
    public bool debug = false;
    [HideInInspector]
    public bool showGrid = true;
    [HideInInspector]
    public Sprite sprite;
    public float rotationZ;
    public GameObject mousePreview;

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
