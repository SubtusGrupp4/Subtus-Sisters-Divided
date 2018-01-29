using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpawnButtonType
{
    public GameObject obj;
    public Vector3 OffSet;
}

public class SpawnButton : BaseButton {

    [Header("Instantiateion")]
    [SerializeField]
    private List<SpawnButtonType> objects = new List<SpawnButtonType>();

    private List<GameObject> allAliveObjects = new List<GameObject>();


    protected override void DoStuff()
    {
        for (int i = 0; i < objects.Count; i++)
        {
           GameObject item = Instantiate(objects[i].obj, transform.position + objects[i].OffSet, Quaternion.identity);
            allAliveObjects.Add(item);
        }
    }

    protected override void UndoStuff()
    {
        for(int i = 0; i < objects.Count; i++)
        {
            Destroy(allAliveObjects[i]);
        }
        allAliveObjects.Clear();
    }
}
