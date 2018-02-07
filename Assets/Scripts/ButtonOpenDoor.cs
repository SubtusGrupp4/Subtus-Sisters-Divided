using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOpenDoor : BaseButton {

    public List<GameObject> objects = new List<GameObject>();


    protected override void DoStuff()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<Door>().Toggle();
        }
    }

    protected override void UndoStuff()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<Door>().Toggle();
        }
        
    }
}
