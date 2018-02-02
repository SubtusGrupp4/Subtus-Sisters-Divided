using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [GiveTag]
    [SerializeField]
    private string[] interactWith = new string[] { };

    private AIMovement movement;

    Vector2 goingTo;

    // Use this for initialization
    void Start()
    {
        movement = GetComponent<AIMovement>();

        //
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        goingTo = movement.GoingWhere();
        goingTo *= movement.speed * Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D obj)
    {
       /* for (int i = 0; i < interactWith.Length; i++)
            if (obj.transform.tag == interactWith[i])
            {
                  obj.transform.position = new Vector3(obj.transform.position.x + goingTo.x, obj.transform.position.y + (goingTo.y > 0 ? 0 : goingTo.y), obj.transform.position.z);
              //  obj.transform.GetComponent<Rigidbody2D>().velocity += new Vector2(goingTo.x, goingTo.y);
            } */
    }

    private void OnTriggerStay2D(Collider2D obj)
    {
        for (int i = 0; i < interactWith.Length; i++)
            if (obj.transform.tag == interactWith[i])
            {
                obj.transform.position = new Vector3(obj.transform.position.x + goingTo.x, obj.transform.position.y + (goingTo.y > 0 ? 0 : goingTo.y), obj.transform.position.z);
                //  obj.transform.GetComponent<Rigidbody2D>().velocity += new Vector2(goingTo.x, goingTo.y);
            }
    }
}
