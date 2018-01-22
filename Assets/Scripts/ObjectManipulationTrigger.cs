using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManipulationTrigger : BaseButton
{

    [Header("Object Manipulation")]

    [SerializeField]
    private List<GameObject> Objs = new List<GameObject>();

    [Header("Rotations")]

    [SerializeField]
    private Vector3 Rotate;
    [SerializeField]
    private float RotateOverTime;


    [Header("Movements")]
    [SerializeField]
    private Vector3 Move;
    [SerializeField]
    private float MoveOverTime;

    private float timer;

    private List<Vector3> ogPos = new List<Vector3>();
    private List<Quaternion> ogRot = new List<Quaternion>();

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < Objs.Count; i++)
        {
            ogPos.Add(Objs[i].transform.position);
            ogRot.Add(Objs[i].transform.rotation);
        }
    }

    private void Update()
    {

    }

    private void Moving()
    {

    }

    protected override void DoStuff()
    {
        timer = MoveOverTime;

        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].GetComponent<Transform>().position += Move;
        }
    }

    protected override void UndoStuff()
    {
        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].GetComponent<Transform>().position -= Move;
        }
    }
}
