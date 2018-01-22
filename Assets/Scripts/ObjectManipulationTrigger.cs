using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectManip
{
    public GameObject Obj;

    [Header("Movements")]
    public Vector3 MoveTo;
    public float MoveTime;
    public float MovingReturnTime;

    [Header("Rotations")]
    public Vector3 RotateTo;
    public float RotationTime;
    public float RotationReturnTime;
    [HideInInspector]
    public bool Moving = false;
    [HideInInspector]
    public bool Returning = false;
}

public class ObjectManipulationTrigger : BaseButton
{
    [Header("Object Manipulation")]

    [SerializeField]
    private List<ObjectManip> Objs = new List<ObjectManip>();

    private List<Vector3> ogPos = new List<Vector3>();
    private List<Quaternion> ogRot = new List<Quaternion>();
    private List<Vector3> tempPos = new List<Vector3>();
    private List<Quaternion> tempRot = new List<Quaternion>();

    private bool moving;
    private float speed;
    private List<float> lerpConstant = new List<float>();
    private List<float> lerpRotConstant = new List<float>();

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < Objs.Count; i++)
        {
            // Main
            ogPos.Add(Objs[i].Obj.transform.position);
            ogRot.Add(Objs[i].Obj.transform.rotation);
            // Temp
            tempPos.Add(Objs[i].Obj.transform.position);
            tempRot.Add(Objs[i].Obj.transform.rotation);

            lerpConstant.Add(0);
            lerpRotConstant.Add(0);
        }


    }

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Rotate()
    {
        for (int i = 0; i < Objs.Count; i++)
        {
            if (Objs[i].Moving)
            {
                //  Debug.Log("moving" + Objs[i].Moving);

                Rotating(
                    Objs[i].Obj, // Obj
                    ogRot[i], // Startpos
                    Quaternion.Euler(Objs[i].RotateTo), // EndPos
                    1 / Objs[i].RotationTime, // Speed
                    i); // Index
            }
            else if (!Objs[i].Moving)
            {
                //   Debug.Log("moving" + Objs[i].Moving);

                Rotating(
                   Objs[i].Obj, // Obj
                   ogRot[i], // Startpos
                   Quaternion.Euler(Objs[i].RotateTo), // EndPos
                   -1 / Objs[i].RotationReturnTime, // Speed
                   i); // Index
            }
        }
    }

    private void Move()
    {
        for (int i = 0; i < Objs.Count; i++)
        {
            if (Objs[i].Moving)
            {

                Moving(
                    Objs[i].Obj, // Obj
                    ogPos[i], // Startpos
                    ogPos[i] + Objs[i].MoveTo, // EndPos
                    Vector3.Distance(ogPos[i], ogPos[i] + Objs[i].MoveTo) / Objs[i].MoveTime, // Speed
                    i); // Index
            }
            else if (!Objs[i].Moving)
            {

                Moving(
                   Objs[i].Obj, // Obj
                   ogPos[i], // Startpos
                   ogPos[i] + Objs[i].MoveTo, // EndPos
                   Vector3.Distance(ogPos[i], ogPos[i] + Objs[i].MoveTo) / Objs[i].MoveTime * -1, // Speed
                   i); // Index
            }
        }
    }

    private void Rotating(GameObject obj, Quaternion startTarget, Quaternion targetPos, float speed, int index)
    {
        if (lerpRotConstant[index] <= 1 && lerpRotConstant[index] >= 0)
            lerpRotConstant[index] += speed * Time.deltaTime;

        if (lerpRotConstant[index] < 0)
            lerpRotConstant[index] = 0;

        if (lerpRotConstant[index] > 1)
            lerpRotConstant[index] = 1;

        Quaternion smoothRot = Quaternion.Lerp(startTarget, targetPos, lerpRotConstant[index]);
        obj.transform.rotation = smoothRot;
    }

    private void Moving(GameObject obj, Vector3 startTarget, Vector3 targetPos, float speed, int index)
    {
        // keep lerp between 0 and 1.
        if (lerpConstant[index] <= 1 && lerpConstant[index] >= 0)
            lerpConstant[index] += speed * Time.deltaTime;

        if (lerpConstant[index] < 0)
            lerpConstant[index] = 0;

        if (lerpConstant[index] > 1)
            lerpConstant[index] = 1;

        Vector3 smoothPos = Vector3.Lerp(startTarget, targetPos, lerpConstant[index]);
        obj.transform.position = smoothPos;

    }

    protected override void DoStuff()
    {
        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].Moving = true;
        }
    }

    protected override void UndoStuff()
    {
        for (int i = 0; i < Objs.Count; i++)
        {

            tempPos[i] = Objs[i].Obj.transform.position;
            tempRot[i] = Objs[i].Obj.transform.rotation;

            Objs[i].Moving = false;
        }
    }
}
