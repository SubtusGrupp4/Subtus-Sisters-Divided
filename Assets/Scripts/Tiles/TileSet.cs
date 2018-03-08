using UnityEngine;
using System.Collections;

public class TileSet : ScriptableObject
{
    public Transform[] prefabs = new Transform[0];
    public bool isStatic = true;
}
