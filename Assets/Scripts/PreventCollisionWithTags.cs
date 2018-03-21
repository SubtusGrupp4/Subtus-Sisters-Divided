using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventCollisionWithTags : MonoBehaviour {

    [GiveTag]
    [SerializeField]
    private string[] tags;
    private BoxCollider2D bcol;

    void Start ()
    {
        bcol = GetComponent<BoxCollider2D>();
        GameObject[] gos = new GameObject[0];

        foreach (string tag in tags)
            gos = GameObject.FindGameObjectsWithTag(tag);

        foreach(GameObject go in gos)
        {
            Collider2D[] cols = go.GetComponents<Collider2D>();
            foreach(Collider2D col in cols)
                Physics2D.IgnoreCollision(col, bcol);
        }
	}
}
